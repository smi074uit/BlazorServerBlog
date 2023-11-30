using SharedModels.Entities.Account;

namespace BlazorServerBlog.Services;

public interface IApiHelper
{
	public Task<bool> login(UserDTO user);
	void SetClientHeaders(string token);
	void BlankClientHeaders();
	void RefreshLogin();
	HttpResponseMessage PostData<T>(T input, string endpoint);
	HttpResponseMessage PostDataString(string input, string endpoint);
	HttpResponseMessage PutData<T>(T input, string endpoint);
	HttpResponseMessage GetData(string endpoint);
	HttpResponseMessage DeleteData(string endpoint);
}