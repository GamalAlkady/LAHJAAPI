using AutoGenerator.Conditions; // Assuming this is part of your base validation framework
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

// Assuming ITValidator interface is defined elsewhere and implies certain capabilities

namespace LAHJAAPI.V1.Validators.Conditions
{
    /// <summary>
    /// Provides a base class for validators with integrated, safe database access using ExecuteInScope.
    /// </summary>
    /// <typeparam name="TContext">The main entity type this validator context operates on.</typeparam>
    /// <typeparam name="EValidator">The enum type defining the validation states.</typeparam>
    public abstract class ValidatorContext<TContext, EValidator> : BaseValidatorContext<TContext, EValidator>, ITValidator
        where TContext : class
        where EValidator : Enum
    {
        // ITFactoryInjector provides access to the DbContextFactory for safe scoped access.
        protected readonly ITFactoryInjector _injector;

        /// <summary>
        /// Initializes a new instance of the ValidatorContext class.
        /// </summary>
        /// <param name="checker">The condition checker providing necessary dependencies.</param>
        public ValidatorContext(IConditionChecker checker) : base(checker)
        {
            // Obtain the injector from the checker, which is provided by the framework.
            _injector = checker.Injector;

            // Ensure injector and its factory are available early.
            if (_injector?.ContextFactory == null)
            {
                // This indicates a configuration issue with the dependency injection setup.
                throw new InvalidOperationException("ITFactoryInjector or its ContextFactory is not initialized. Ensure the ConditionChecker is correctly configured in the DI container.");
            }
        }

        /// <summary>
        /// Executes a database query returning a list of entities within a safe, scoped DbContext.
        /// This is the preferred way to query DbSet for multiple items.
        /// </summary>
        /// <typeparam name="T">The type of entity to query.</typeparam>
        /// <param name="query">A function defining the LINQ query on the DbSet.</param>
        /// <returns>A task representing the asynchronous operation, returning a list of entities.</returns>
        protected Task<List<T>> QueryListAsync<T>(Func<DbSet<T>, IQueryable<T>> query)
            where T : class
        {
            // Access DbContextFactory via the injector to create a scope.
            return _injector.ContextFactory.ExecuteInScopeAsync(ctx =>
                query(ctx.Set<T>()).AsNoTracking().ToListAsync()
            );
        }

        /// <summary>
        /// Executes a database query returning the count of entities within a safe, scoped DbContext.
        /// </summary>
        /// <typeparam name="T">The type of entity to count.</typeparam>
        /// <param name="query">A function defining the LINQ query on the DbSet.</param>
        /// <returns>A task representing the asynchronous operation, returning the count of entities.</returns>
        protected Task<int> QueryCountAsync<T>(Func<DbSet<T>, IQueryable<T>> query)
            where T : class
        {
            return _injector.ContextFactory.ExecuteInScopeAsync(ctx =>
                query(ctx.Set<T>()).CountAsync()
            );
        }


        protected Task<int> QueryCountAsync<T>(Expression<Func<T, bool>> predicate)
            where T : class
        {
            return _injector.ContextFactory.ExecuteInScopeAsync(ctx => ctx.Set<T>().CountAsync(predicate));
        }

        /// <summary>
        /// Executes a database query returning true if ANY entity matches the condition within a safe, scoped DbContext.
        /// </summary>
        /// <typeparam name="T">The type of entity to check.</typeparam>
        /// <param name="query">A function defining the LINQ query (typically with a Where clause) on the DbSet.</param>
        /// <returns>A task representing the asynchronous operation, returning true if any entity matches the query.</returns>
        protected Task<bool> QueryAnyAsync<T>(Func<DbSet<T>, IQueryable<T>> query)
             where T : class
        {
            return _injector.ContextFactory.ExecuteInScopeAsync(ctx =>
                 query(ctx.Set<T>()).AnyAsync()
             );
        }


        /// <summary>
        /// Executes a database query returning true if ALL entities match the condition within a safe, scoped DbContext.
        /// </summary>
        /// <typeparam name="T">The type of entity to check.</typeparam>
        /// <param name="predicate">A expression defining the LINQ query (typically with a Where clause) on the DbSet.</param>
        /// <returns>A task representing the asynchronous operation, returning true if all entities match the query.</returns>
        protected Task<bool> QueryAllAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return _injector.ContextFactory.ExecuteInScopeAsync(ctx =>
                ctx.Set<T>().AllAsync(predicate)
            );
        }


        /// <summary>
        /// Executes a database query returning the first entity matching the query, or null if none found,
        /// within a safe, scoped DbContext. This is preferred for fetching a single potential item.
        /// </summary>
        /// <typeparam name="T">The type of entity to query.</typeparam>
        /// <param name="query">A function defining the LINQ query (typically with a Where clause) on the DbSet.</param>
        /// <returns>A task representing the asynchronous operation, returning the first matching entity or null.</returns>
        // Renamed from QuerySingleAsync for clarity (it uses FirstOrDefaultAsync)
        //protected Task<T?> QueryFirstOrDefaultAsync<T>(Func<DbSet<T>, IQueryable<T>> query)
        protected Task<T?> QueryFirstOrDefaultAsync<T>(Expression<Func<T, bool>> predicate)
            where T : class
        {
            return _injector.ContextFactory.ExecuteInScopeAsync(ctx =>
                ctx.Set<T>().FirstOrDefaultAsync(predicate)
            );
        }

        /// <summary>
        /// Finds the main context entity (<typeparamref name="TContext"/>) by its ID using DbContext.FindAsync
        /// within a safe, scoped DbContext. This is often efficient if the entity is cached.
        /// </summary>
        /// <param name="id">The ID of the entity to find.</param>
        /// <returns>A task representing the asynchronous operation, returning the entity or null.</returns>
        protected virtual Task<TContext?> FindContextEntityAsync(params string[]? id) // Renamed from FindModel
        {
            return _injector.ContextFactory.ExecuteInScopeAsync(ctx =>
                // FindAsync is suitable for finding by primary key directly
                ctx.Set<TContext>().FindAsync(id).AsTask()
            );
        }

        /// <summary>
        /// Provides the framework's default method for retrieving the main context entity (<typeparamref name="TContext"/>)
        /// by ID. By default, this calls FindContextEntityAsync.
        /// </summary>
        /// <param name="id">The ID of the entity to get.</param>
        /// <returns>A task representing the asynchronous operation, returning the entity or null.</returns>
        // Overrides the base class method. Kept original name as it's part of the base framework contract.
        protected override Task<TContext?> GetModel(string? id)
        {
            // Delegate to the more specific and descriptive helper method.
            return FindContextEntityAsync(id);
        }


        /// <summary>
        /// Finds an entity of a specific type <typeparamref name="T"/> by its ID using DbContext.FindAsync
        /// within a safe, scoped DbContext. Useful for looking up related entities by primary key.
        /// </summary>
        /// <typeparam name="T">The type of entity to find.</typeparam>
        /// <param name="id">The ID of the entity to find.</param>
        /// <returns>A task representing the asynchronous operation, returning the entity or null.</returns>
        protected Task<T?> FindEntityByIdAsync<T>(string? id) where T : class
        {
            if (string.IsNullOrWhiteSpace(id)) return Task.FromResult<T?>(null);

            return _injector.ContextFactory.ExecuteInScopeAsync(ctx =>
                ctx.Set<T>().FindAsync(id).AsTask()
            );
        }


    }
}