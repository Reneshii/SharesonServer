using SharesonServer.Model.Support;
using SharesonServer.Repository.SupportFunctions;
using System.Collections.Generic;

namespace SharesonServer.Interface
{
    public interface IMainMenuRepositoryFunctionsWithoutLimit : IMainMenuRepository
    {
        SqlHelper sql { get; set; }
        List<FullClientInfoModel> UpdateUsersList();
    }
}
