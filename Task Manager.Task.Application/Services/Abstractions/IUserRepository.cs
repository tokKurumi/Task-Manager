using Task_Manager.Common;
using Task_Manager.Task.Core.Entities;

namespace Task_Manager.Task.Application.Services.Abstractions;

public interface IUserRepository : IGenericRepository<User, UserRepositoryError>;

public abstract record UserRepositoryError : IError;
