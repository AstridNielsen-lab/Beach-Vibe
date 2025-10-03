using Supabase;

namespace BeachApp_Cliente.Data
{
    public static class SupabaseConfig
    {
        public static string Url => "https://givgbgiynnkmxpxojthx.supabase.co";
        public static string AnonKey => "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImdpdmdiZ2l5bm5rbXhweG9qdGh4Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3NTkxNzU0MTUsImV4cCI6MjA3NDc1MTQxNX0.xjIhuzeTEKI4YhWOoWrZ5p76kUN8IYbsVhpvZa1jr00";

        public static SupabaseOptions GetOptions()
        {
            return new SupabaseOptions
            {
                AutoConnectRealtime = true
            };
        }
    }
}
