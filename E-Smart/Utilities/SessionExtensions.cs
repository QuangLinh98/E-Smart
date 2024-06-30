using Newtonsoft.Json;

namespace E_Smart.Utilities
{
	public static class SessionExtensions
	{
		//Phương thức này có chức năng là lưu trữ một đối tượng kiểu T vào Session với một key nhất định.
		//Phương thức này cho phép bạn lưu trữ một đối tượng của bất kỳ kiểu nào (generic type T) vào Session trong ASP.NET Core một cách dễ dàng.
		public static void SetJson(this ISession session, string key, object value)
		{
			// Session này sẽ Set 1 cái mảng (key và value)
			session.SetString(key, JsonConvert.SerializeObject(value));
		}


		//Phương thức này có chức năng là truy xuất và chuyển đổi một đối tượng từ chuỗi JSON lưu trữ trong Session về kiểu dữ liệu T.
		public static T GetJson<T>(this ISession session, string key)
		{
			var sessionData = session.GetString(key);
			return sessionData == null ? default(T) : JsonConvert.DeserializeObject<T>(sessionData);
		}
	}
}
