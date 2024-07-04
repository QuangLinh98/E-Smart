using E_Smart.Areas.Client.Models.ViewModels;

namespace E_Smart.Service
{
	public interface IVnPayService
	{
		string CreatePaymentUrl(HttpContext context , VnPaymentRequestModel model);
		VnPaymentResponseViewModel PaymentExecute(IQueryCollection collections);
	}
}
