namespace BookMyShow.Persistence.Constants;

public static class SeedDataIds
{
    public static class Roles
    {
        public static readonly Guid SuperAdmin = Guid.Parse("11111111-1111-1111-1111-111111111111");
        public static readonly Guid VenueAdmin = Guid.Parse("22222222-2222-2222-2222-222222222222");
        public static readonly Guid Organizer = Guid.Parse("33333333-3333-3333-3333-333333333333");
        public static readonly Guid Customer = Guid.Parse("44444444-4444-4444-4444-444444444444");
    }

    public static class Genres
    {
        public static readonly Guid Action = Guid.Parse("10000000-0000-0000-0000-000000000001");
        public static readonly Guid Comedy = Guid.Parse("10000000-0000-0000-0000-000000000002");
        public static readonly Guid Drama = Guid.Parse("10000000-0000-0000-0000-000000000003");
        public static readonly Guid Horror = Guid.Parse("10000000-0000-0000-0000-000000000004");
        public static readonly Guid Thriller = Guid.Parse("10000000-0000-0000-0000-000000000005");
        public static readonly Guid SciFi = Guid.Parse("10000000-0000-0000-0000-000000000006");
        public static readonly Guid Romance = Guid.Parse("10000000-0000-0000-0000-000000000007");
    }

    public static class Languages
    {
        public static readonly Guid English = Guid.Parse("20000000-0000-0000-0000-000000000001");
        public static readonly Guid Hindi = Guid.Parse("20000000-0000-0000-0000-000000000002");
        public static readonly Guid Tamil = Guid.Parse("20000000-0000-0000-0000-000000000003");
        public static readonly Guid Telugu = Guid.Parse("20000000-0000-0000-0000-000000000004");
        public static readonly Guid Malayalam = Guid.Parse("20000000-0000-0000-0000-000000000005");
    }

    public static class SeatCategories
    {
        public static readonly Guid Vip = Guid.Parse("30000000-0000-0000-0000-000000000001");
        public static readonly Guid Platinum = Guid.Parse("30000000-0000-0000-0000-000000000002");
        public static readonly Guid Gold = Guid.Parse("30000000-0000-0000-0000-000000000003");
        public static readonly Guid Silver = Guid.Parse("30000000-0000-0000-0000-000000000004");
    }
}
