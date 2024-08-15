using DataAccessObject.Enums;

namespace BusinessObject.Mappers;

public class StatusMapper
{
    public static string MapPaymentStatusToTransactionStatus(string paymentStatus)
    {
        return paymentStatus switch
        {
            "Successful" => TransactionStatus.COMPLETED,
            "Failed" => TransactionStatus.FAILED,
            "Cancelled" => TransactionStatus.CANCELLED,
            "Expired" => TransactionStatus.FAILED, 
            "Refund" => TransactionStatus.CANCELLED, 
            _ => TransactionStatus.PENDING, 
        };
    }
}