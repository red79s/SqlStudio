using Common.Model;

namespace Common
{
    public interface IExecuter
    {
        SqlResult Execute(string sqlQuery);
    }
}
