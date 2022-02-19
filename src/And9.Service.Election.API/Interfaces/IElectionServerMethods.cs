namespace And9.Service.Election.API.Interfaces;

public interface IElectionServerMethods
{
    Task<bool> Register();
    Task<bool> CancelRegister();
    Task<bool> Vote();
    Task<Abstractions.Models.Election> GetElection();

}