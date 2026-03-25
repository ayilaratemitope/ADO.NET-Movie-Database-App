using System;
using System.Data;

namespace ADOMovieDatabaseConsoleApp
{
    internal class Movie
    {
         public int Id { get; set; }
         public string Title { get; set; }
         public int ReleaseYear { get; set; }
         public string Genre { get; set; }

        #region MaterialiseFromReader note
        // static method to "materialise" a row from a data reader back to a movie object
        // note how the referenc to the reader object is IDataReader and not SqlDataReader
        // this is because I want to keep this method as generic as possible and 
        // SqlDataReader only works with MS SQL Server databases.
        // There are other readers that implement IDataReader such as MySqlDataReader 
        // for MySQL servers and thus this method should work equally well for both of them!
        #endregion
        public static Movie MaterialiseFromReader(IDataReader reader)
        {
            var movie = new Movie()
            {
                Id = int.Parse(reader[nameof(Id)].ToString()),
                Title = reader[nameof(Title)].ToString(),
                ReleaseYear = int.Parse(reader[nameof(ReleaseYear)].ToString()),
                Genre = reader[nameof(Genre)].ToString()
            };
            return movie;
        }
        public void DematerialiseToDbCommand<TDbDataParameter>(IDbCommand command)
            where TDbDataParameter : IDbDataParameter, new()
        {
            #region TDbDataParameter note
            // normally in this method you'd just have lines like this which copy the values from the object into SQL parameters for the query:
            //
            // command.Parameters.Add(new SqlParameter("@Title", movie.Title));
            // ...
            //
            // but because I don't want to tie this class only to MS SQL Server, I need to do the above, 
            // where I pass the SqlParameter class in as a generic (template) type parameter called TDbDataParameter
            // and I say that TDbDataParameter must be a class type that implements the IDbDataParameter interface 
            // and has a default parameterless constructor by using this generic method constraint:
            //
            // where TDbDataParameter : IDbDataParameter, new()
            //
            // Then, because I'm using the SqlParameter class generically, 
            // I have to initialise it a little differently when created like this:
            // 
            // command.Parameters.Add(new TDbDataParameter() { ParameterName = "@Title", Value = this.Title });
            //
            // Don't worry if you don't understand how this works fully, 
            // it's a very advanced style of programming designed to promote code reuse!
            #endregion

            command.Parameters.Add(new TDbDataParameter() { ParameterName = "@Title", Value = this.Title });
            command.Parameters.Add(new TDbDataParameter() { ParameterName = "@ReleaseYear", Value = this.ReleaseYear });
            command.Parameters.Add(new TDbDataParameter() { ParameterName = "@Genre", Value = this.Genre });
        } 
    }
}
