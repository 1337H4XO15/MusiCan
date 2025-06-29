namespace MusiCan.Server.Helper
{
    public enum Roles : byte
    {
        Admin = 0,
        Kuenstler = 1,
        Nutzer = 2,
        Banned = 3
    }

    public enum Access : byte
    {
        None = 0,
        Owner = 1,
        Shared = 2
    }
}
