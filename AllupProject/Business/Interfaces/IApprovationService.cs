namespace AllupProject.Business.Interfaces;

public interface IApprovationService
{
    Task NotApprove(int orderId);
    Task Approve(int orderId);
}
