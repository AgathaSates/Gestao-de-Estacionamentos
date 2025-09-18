using Gestao_de_Estacionamentos.Core.Dominio.Compartilhado;
using Microsoft.EntityFrameworkCore;

namespace Gestao_de_Estacionamentos.Infraestutura.Orm.Compartilhado;

public class RepositorioBaseEmOrm<T> where T : EntidadeBase<T>
{
    protected readonly DbSet<T> registros;

    public RepositorioBaseEmOrm(AppDbContext contexto)
    {
        registros = contexto.Set<T>();
    }

    public virtual async Task CadastrarAsync(T novoRegistro)
    {
        await registros.AddAsync(novoRegistro);
    }

    public virtual async Task CadastrarEntidadesAsync(IList<T> entidades)
    {
        await registros.AddRangeAsync(entidades);
    }

    public virtual async Task<T> EditarAsync(Guid idRegistro, T registroEditado)
    {
        var registroSelecionado = await SelecionarRegistroPorIdAsync(idRegistro);

        if (registroSelecionado is null)
            return null!;

        registroSelecionado.AtualizarRegistro(registroEditado);

        return registroSelecionado;
    }

    public async Task<bool> ExcluirAsync(Guid idRegistro)
    {
        var registroSelecionado = await SelecionarRegistroPorIdAsync(idRegistro);

        if (registroSelecionado is null)
            return false;

        registros.Remove(registroSelecionado);

        return true;
    }

    public virtual async Task<T?> SelecionarRegistroPorIdAsync(Guid idRegistro)
    {
        return await registros.FirstOrDefaultAsync(x => x.Id.Equals(idRegistro));
    }

    public virtual async Task<List<T>> SelecionarRegistrosAsync()
    {
        return await registros.ToListAsync();
    }
    public virtual async Task<List<T>> SelecionarRegistrosAsync(int quantidade)
    {
        return await registros.Take(quantidade).ToListAsync();
    }
}