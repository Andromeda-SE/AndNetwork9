namespace And9.Lib.Broker.Crud;

[Flags]
public enum CrudFlags
{
    None = 0,

    Create = 1,
    Read = 2,
    Update = 4,
    Delete = 8,

    All = Create | Read | Update | Delete,
}