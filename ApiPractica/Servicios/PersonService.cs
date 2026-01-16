using ApiPractica.Interfaces;
using ApiPractica.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiPractica.Servicios
{
    public class PersonService(PersonDbContext context) : IPersonService
    {
        private readonly PersonDbContext _context = context;
        private readonly DbSet<Person> _dbSet = context.Set<Person>();

        public async Task<int> AddUser(Person person)
        {
            await _context.AddAsync(person);

            int filasAfectadas = await _context.SaveChangesAsync();

            return filasAfectadas;
        }

        public async Task<int> DeleteUser(int id)
        {
            var person = await _context.Persons.FindAsync(id);
            if (person == null) 
            {
                return 0;
            }

            _context.Persons.Remove(person);

            return await _context.SaveChangesAsync();
        }

        public async Task<List<Person>> GetAll()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<Person?> GetById(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<int> UpdateUser(Person person)
        {
            _context.Persons.Update(person);

            return await _context.SaveChangesAsync();
        }
    }
}
