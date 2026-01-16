using ApiPractica.Models;

namespace ApiPractica.Interfaces
{
    public interface IPersonService
    {
        Task<List<Person>> GetAll();
        Task<Person?> GetById(int id);
        Task<int> AddUser(Person person);
        Task<int> UpdateUser(Person person);
        Task<int> DeleteUser(int id);
    }
}
