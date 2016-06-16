namespace Wibci.Azure.AppService.Auth.DataAccess
{
    public abstract class EntityDataAccessBase
    {
        private AuthIdentityDbContext _context;

        public EntityDataAccessBase()
        {
            //TODO: IoC DataAccessContext
            _context = new DefaultDbContextProvider().Context();
        }

        public EntityDataAccessBase(AuthIdentityDbContext context)
        {
            //NOTE: To share data context, use this constructor
            _context = context;
        }

        protected AuthIdentityDbContext Context
        {
            get { return _context; }
            private set { _context = value; }
        }

        public void Dispose()
        {
            Context.Dispose();
        }

        public void SaveChanges()
        {
            Context.SaveChanges();
        }

        protected void DeleteRecordDisconnectState<T>(T deleteRecord) where T : class
        {
            Context.Entry(deleteRecord).State = System.Data.Entity.EntityState.Deleted;
            Context.SaveChanges();
        }

        protected void UpdateDetachedRecord<T>(T updateRecord) where T : class
        {
            //consider auto conflict resolution: https://lukescodedump.wordpress.com/2012/01/13/automatic-concurrency-conflict-resolution-with-entity-framework-code-first/
            //Context.Entry(updateRecord).CurrentValues.SetValues(dynmicCopy); //doesn't work because Id is part of update record - need DTO in this scenario
            Context.Entry(updateRecord).State = System.Data.Entity.EntityState.Modified; // all properties are supposed to be sent to db
            Context.SaveChanges();
        }
    }
}