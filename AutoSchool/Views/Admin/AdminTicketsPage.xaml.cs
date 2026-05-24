using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace AutoSchool.Views.Admin
{
    public partial class AdminTicketsPage : Page
    {
        public ObservableCollection<string> Topics { get; } = new();

        public string SelectedTopic
        {
            get => (string)GetValue(SelectedTopicProperty);
            set => SetValue(SelectedTopicProperty, value);
        }

        public static readonly DependencyProperty SelectedTopicProperty =
            DependencyProperty.Register(nameof(SelectedTopic), typeof(string), typeof(AdminTicketsPage),
                new PropertyMetadata("Все темы", (d, e) =>
                {
                    if (d is AdminTicketsPage p)
                        p._ticketsView?.View?.Refresh();
                }));

        private CollectionViewSource? _ticketsView;
        private IEnumerable? _ticketsSource;
        private INotifyCollectionChanged? _ticketsNcc;

        private object? _arg1;
        private object? _arg2;

        public AdminTicketsPage()
        {
            InitializeComponent();
            Loaded += (_, __) => Initialize();
            Unloaded += (_, __) => UnhookTickets();
        }

        public AdminTicketsPage(object? arg1) : this(arg1, null) { }

        public AdminTicketsPage(object? arg1, object? arg2) : this()
        {
            _arg1 = arg1;
            _arg2 = arg2;

            if (LooksLikeViewModel(arg1)) DataContext = arg1;
            else if (LooksLikeViewModel(arg2)) DataContext = arg2;
        }

        private static bool LooksLikeViewModel(object? obj)
            => obj != null && obj.GetType().Name.EndsWith("ViewModel", StringComparison.OrdinalIgnoreCase);

        private void Initialize()
        {
            _ticketsView = (CollectionViewSource)Resources["TicketsView"];

            // если DataContext не передали, пробуем создать VM (если он у тебя называется стандартно)
            if (DataContext == null)
            {
                var vm = TryCreateTicketsViewModel(_arg1, _arg2);
                if (vm != null) DataContext = vm;
            }

            if (DataContext == null)
                return;

            // просим VM загрузить данные из БД (если он умеет)
            TryCallLoad(DataContext);

            // подключаем Tickets как источник
            HookTicketsFromVm();

            // темы: сначала из VM (как у пользователя), иначе из билетов
            RebuildTopics();

            if (!Topics.Contains(SelectedTopic))
                SelectedTopic = "Все темы";

            _ticketsView.View?.Refresh();
        }

        private void HookTicketsFromVm()
        {
            UnhookTickets();

            if (_ticketsView == null || DataContext == null) return;

            var ticketsProp = DataContext.GetType().GetProperty("Tickets", BindingFlags.Instance | BindingFlags.Public);
            if (ticketsProp == null) return;

            _ticketsSource = ticketsProp.GetValue(DataContext) as IEnumerable;
            _ticketsView.Source = _ticketsSource;

            _ticketsNcc = _ticketsSource as INotifyCollectionChanged;
            if (_ticketsNcc != null)
                _ticketsNcc.CollectionChanged += Tickets_CollectionChanged;
        }

        private void UnhookTickets()
        {
            if (_ticketsNcc != null)
                _ticketsNcc.CollectionChanged -= Tickets_CollectionChanged;

            _ticketsNcc = null;
            _ticketsSource = null;
        }

        private void Tickets_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            // если билеты обновились (после загрузки/добавления) — пересобрать темы
            RebuildTopics();
            _ticketsView?.View?.Refresh();
        }

        private void RebuildTopics()
        {
            Topics.Clear();
            Topics.Add("Все темы");

            // 1) Пытаемся взять темы напрямую из VM (идеально: совпадает с пользовательской частью)
            var gotFromVm = TryFillTopicsFromVm(DataContext);
            if (gotFromVm)
            {
                EnsureSelectedTopicValid();
                return;
            }

            // 2) Иначе строим темы из Tickets (как минимум совпадёт с темами, по которым есть билеты)
            if (_ticketsSource != null)
            {
                var set = _ticketsSource
                    .Cast<object>()
                    .Select(GetTopicName)
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(s => s);

                foreach (var t in set)
                    Topics.Add(t);
            }

            EnsureSelectedTopicValid();
        }

        private void EnsureSelectedTopicValid()
        {
            if (string.IsNullOrWhiteSpace(SelectedTopic) || !Topics.Contains(SelectedTopic))
                SelectedTopic = "Все темы";
        }

        // ==== фильтр DataGrid по теме ====
        private void TicketsView_Filter(object sender, FilterEventArgs e)
        {
            if (e.Item == null) { e.Accepted = false; return; }

            var selected = SelectedTopic;
            if (string.IsNullOrWhiteSpace(selected) || selected == "Все темы")
            {
                e.Accepted = true;
                return;
            }

            var topic = GetTopicName(e.Item);
            e.Accepted = string.Equals(topic, selected, StringComparison.OrdinalIgnoreCase);
        }

        // ==== Получение названия темы из Ticket (универсально) ====
        private static string GetTopicName(object item)
        {
            var t = item.GetType();

            // 1) строковые свойства
            foreach (var propName in new[] { "TopicName", "TopicTitle", "ThemeName" })
            {
                var p = t.GetProperty(propName, BindingFlags.Instance | BindingFlags.Public);
                if (p?.GetValue(item) is string s && !string.IsNullOrWhiteSpace(s))
                    return s;
            }

            // 2) вложенный объект Topic/Theme с Title/Name
            foreach (var objName in new[] { "Topic", "Theme" })
            {
                var pObj = t.GetProperty(objName, BindingFlags.Instance | BindingFlags.Public);
                var obj = pObj?.GetValue(item);
                if (obj == null) continue;

                var ot = obj.GetType();
                foreach (var nameProp in new[] { "Title", "Name" })
                {
                    var pName = ot.GetProperty(nameProp, BindingFlags.Instance | BindingFlags.Public);
                    if (pName?.GetValue(obj) is string s2 && !string.IsNullOrWhiteSpace(s2))
                        return s2;
                }
            }

            return "Без темы";
        }

        // ==== Темы напрямую из VM (как у пользователя) ====
        private bool TryFillTopicsFromVm(object? vm)
        {
            if (vm == null) return false;
            var t = vm.GetType();

            // A) свойство Topics
            var pTopics = t.GetProperty("Topics", BindingFlags.Instance | BindingFlags.Public);
            if (pTopics?.GetValue(vm) is IEnumerable topicsEnum)
            {
                FillFromTopicsEnumerable(topicsEnum);
                return Topics.Count > 1;
            }

            // B) метод GetTopics()
            var m = t.GetMethod("GetTopics", BindingFlags.Instance | BindingFlags.Public);
            if (m != null && m.GetParameters().Length == 0 && m.Invoke(vm, null) is IEnumerable enum2)
            {
                FillFromTopicsEnumerable(enum2);
                return Topics.Count > 1;
            }

            return false;
        }

        private void FillFromTopicsEnumerable(IEnumerable topicsEnum)
        {
            foreach (var topic in topicsEnum)
            {
                if (topic == null) continue;

                // Topic может быть строкой или объектом с Name/Title
                if (topic is string s)
                {
                    if (!string.IsNullOrWhiteSpace(s) && !Topics.Contains(s))
                        Topics.Add(s);
                    continue;
                }

                var tt = topic.GetType();
                foreach (var nameProp in new[] { "Title", "Name", "TopicName" })
                {
                    var p = tt.GetProperty(nameProp, BindingFlags.Instance | BindingFlags.Public);
                    if (p?.GetValue(topic) is string s2 && !string.IsNullOrWhiteSpace(s2))
                    {
                        if (!Topics.Contains(s2))
                            Topics.Add(s2);
                        break;
                    }
                }
            }

            // сортировка (кроме "Все темы")
            var all = Topics.Skip(1).OrderBy(x => x).ToList();
            Topics.Clear();
            Topics.Add("Все темы");
            foreach (var x in all) Topics.Add(x);
        }

        // ==== создание VM, если навигация не передала (оставлено как страховка) ====
        private static object? TryCreateTicketsViewModel(object? arg1, object? arg2)
        {
            try
            {
                var asm = Assembly.GetExecutingAssembly();
                var vmType = asm.GetType("AutoSchool.ViewModels.Admin.AdminTicketsViewModel");
                if (vmType == null) return null;

                // ctor(arg1,arg2) / ctor(arg2,arg1)
                if (arg1 != null && arg2 != null)
                {
                    var obj = CreateByArgs(vmType, arg1, arg2) ?? CreateByArgs(vmType, arg2, arg1);
                    if (obj != null) return obj;
                }

                // ctor(arg1)
                if (arg1 != null)
                {
                    var obj = CreateByArgs(vmType, arg1);
                    if (obj != null) return obj;
                }

                // ctor()
                var empty = vmType.GetConstructor(Type.EmptyTypes);
                if (empty != null) return Activator.CreateInstance(vmType);
            }
            catch { }

            return null;
        }

        private static object? CreateByArgs(Type t, params object[] args)
        {
            try
            {
                foreach (var ctor in t.GetConstructors())
                {
                    var ps = ctor.GetParameters();
                    if (ps.Length != args.Length) continue;

                    bool ok = true;
                    for (int i = 0; i < ps.Length; i++)
                        if (args[i] == null || !ps[i].ParameterType.IsInstanceOfType(args[i])) { ok = false; break; }

                    if (!ok) continue;
                    return ctor.Invoke(args);
                }
            }
            catch { }

            return null;
        }

        // ==== просим VM загрузить данные из БД ====
        private static void TryCallLoad(object vm)
        {
            var t = vm.GetType();

            foreach (var name in new[] { "Load", "Reload", "Refresh", "LoadTickets" })
            {
                var m = t.GetMethod(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                    binder: null, types: Type.EmptyTypes, modifiers: null);

                if (m != null)
                {
                    try { m.Invoke(vm, null); } catch { }
                    return;
                }
            }

            foreach (var propName in new[] { "LoadCommand", "RefreshCommand", "ReloadCommand" })
            {
                var p = t.GetProperty(propName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (p?.GetValue(vm) is ICommand cmd && cmd.CanExecute(null))
                {
                    try { cmd.Execute(null); } catch { }
                    return;
                }
            }
        }
    }
}