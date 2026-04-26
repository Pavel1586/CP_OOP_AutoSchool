using AutoSchool.Models;

namespace AutoSchool.Services
{
    public static class UserSession
    {
        public static User? CurrentUser { get; set; }
    }
}