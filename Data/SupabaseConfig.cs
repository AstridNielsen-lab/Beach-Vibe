using Supabase;

namespace AdmBeachApp.Data
{
    public static class SupabaseConfig
    {
        public static string Url => "https://givgbgiynnkmxpxojthx.supabase.co";
        public static string AnonKey => "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImdpdmdiZ2l5bm5rbXhweG9qdGh4Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3NTkxNzU0MTUsImV4cCI6MjA3NDc1MTQxNX0.xjIhuzeTEKI4YhWOoWrZ5p76kUN8IYbsVhpvZa1jr00";
        
        // Informações de conexão PostgreSQL para desenvolvimento
        public static string DatabaseUrl => "postgresql://postgres:Julio###78451200@db.givgbgiynnkmxpxojthx.supabase.co:5432/postgres";
        public static string PoolerUrl => "postgresql://postgres.givgbgiynnkmxpxojthx:Julio###78451200@aws-1-us-east-2.pooler.supabase.com:6543/postgres";

        public static SupabaseOptions GetOptions()
        {
            return new SupabaseOptions
            {
                AutoConnectRealtime = true,
                AutoRefreshToken = true
            };
        }
    }
}
