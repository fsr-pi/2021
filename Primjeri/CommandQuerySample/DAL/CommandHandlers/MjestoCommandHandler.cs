using CommandQueryCore;
using Contract.Commands;
using DAL.Models;
using System;
using System.Threading.Tasks;

namespace DAL.CommandHandlers
{
  public class MjestoCommandHandler : ICommandHandler<DeleteMjesto>, ICommandHandler<AddMjesto, int>, ICommandHandler<UpdateMjesto>
  {
    private readonly FirmaContext ctx;    

    public MjestoCommandHandler(FirmaContext ctx)
    {
      this.ctx = ctx;
    }   

    public async Task Handle(DeleteMjesto command)
    {
      var drzava = await ctx.Mjesto.FindAsync(command.Id);
      if (drzava != null)
      {       
        ctx.Remove(drzava);
        await ctx.SaveChangesAsync();        
      }
      else
      {
        throw new ArgumentException($"Nepostojeća oznaka mjesta: {command.Id}");
      }
    }

    public async Task<int> Handle(AddMjesto command)
    {
      var mjesto = new Mjesto
      {
        NazMjesta = command.NazivMjesta,
        PostBrMjesta = command.PostBrojMjesta,
        PostNazMjesta = command.PostNazivMjesta,
        OznDrzave = command.OznDrzave
      };
      ctx.Add(mjesto);
      await ctx.SaveChangesAsync();
      return mjesto.IdMjesta;
    }

    public async Task Handle(UpdateMjesto command)
    {
      var mjesto = await ctx.Mjesto.FindAsync(command.IdMjesta);
      if (mjesto != null)
      {
        mjesto.NazMjesta = command.NazivMjesta;
        mjesto.PostBrMjesta = command.PostBrojMjesta;
        mjesto.PostNazMjesta = command.PostNazivMjesta;
        mjesto.OznDrzave = command.OznDrzave;
        await ctx.SaveChangesAsync();
      }
      else 
        throw new ArgumentException($"Nepostojeća oznaka mjesta: {command.IdMjesta}");
    }
  }
}
