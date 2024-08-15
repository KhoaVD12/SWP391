using System.Runtime.Serialization;

namespace DataAccessObject.Enums;

public class Status
{
public static readonly string ACTIVE = "Active";
public static readonly string INACTIVE = "Inactive";
}

public class TransactionStatus
{
    public static readonly string PENDING = "Pending";
    public static readonly string COMPLETED = "Completed";
    public static readonly string FAILED = "Failed";
    public static readonly string CANCELLED = "cancelled";
}

public class PaymentStatus
{
    public static readonly string PENDING = "Pending";
    public static readonly string SUCCESSFUL = "Successful";
    public static readonly string FAILED = "Failed";
    public static readonly string CANCELLED = "Cancelled";
    public static readonly string EXPIRED = "Expired";
    public static readonly string REFUND = "Refund";
}
