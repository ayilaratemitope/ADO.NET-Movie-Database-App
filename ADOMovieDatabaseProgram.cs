using System.Configuration;
using Microsoft.Data.SqlClient;

namespace ADOMovieDatabaseConsoleApp
{
    internal class ADOMovieDatabaseProgram
    {
        static void Main(string[] args)
        {
            int? menuChoice;

            do
            {
                DisplayMenu();
                menuChoice = PromptForMenuChoice();

                switch (menuChoice)
                {
                    case 1:
                        AddMovie();
                        break;

                    case 2:
                        ListAllMovies();
                        break;

                    case 3:
                        FindMovieByTitle();
                        break;

                    case 4:
                        FindMovieById();
                        break;

                    case 5:
                        DeleteMovieById();
                        break;

                    case 6:
                        UpdateMovieById();
                        break;
                }
            } while (menuChoice != 0);      
        }
        public static void DisplayMenu()
        {
            Console.WriteLine("Movie Database Main Menu");
            Console.WriteLine("========================" + Environment.NewLine);
            Console.WriteLine("1. Add Movie");
            Console.WriteLine("2. List All Movies");
            Console.WriteLine("3. Find Movie By Title");
            Console.WriteLine("4. Find Movie By Id");
            Console.WriteLine("5. Delete Movie By Id");
            Console.WriteLine("6. Update Movie By Id");
            Console.WriteLine($"0. Exit{Environment.NewLine}");
        }
        public static int? PromptForMenuChoice()
        {
            Console.Write("Input menu choice number: ");
            var inputOk = int.TryParse(Console.ReadLine(), out int choice);
            Console.WriteLine();
            return  inputOk ? choice : null;    
        }

        public static void ListAllMovies()
        {
            // Create the connection which will talk to the database via SQL
            using (SqlConnection connection =
                new SqlConnection(ConfigurationManager.ConnectionStrings["ADOMovieConnectionString"].ConnectionString))
            {
                // Create an SqlCommand, note here we're embedding SQL statements as text strings into our code
                using (SqlCommand sqlCommand =
                    new SqlCommand("SELECT Id, Title, ReleaseYear, Genre FROM Movie ORDER BY Title;", connection))
                {
                    try
                    {
                        // open connection
                        connection.Open();

                        // run the query
                        SqlDataReader reader = sqlCommand.ExecuteReader();

                        // enumerate query result and get back one date one database row for 
                        // each pass through this while loop
                        while (reader.Read())
                        {
                            var movie = Movie.MaterialiseFromReader(reader);

                            Console.WriteLine($"Id: {movie.Id}");
                            Console.WriteLine($"Movie Name: {movie.Title}");
                            Console.WriteLine($"Release Year: {movie.ReleaseYear}");
                            Console.WriteLine($"Genre: {movie.Genre}{Environment.NewLine}");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Something went wrong trying to store data to the database: {e.Message}");
                    }
                    finally
                    {
                        connection.Close();
                    }

                    Console.WriteLine("Press a key!");
                    Console.ReadKey();
                }
            }
        }

        public static void AddMovie()
        {
            // declare local var for new Movie object
            var movie = new Movie();

            // read values from user and populate properties of movie object with them
            Console.Write("Title? ");
            movie.Title = Console.ReadLine();

            Console.Write("Year Released? ");
            movie.ReleaseYear = int.Parse(Console.ReadLine());

            Console.Write("Genre? ");
            movie.Genre = Console.ReadLine();

            // Create the connection which will talk to the database via SQL
            using (SqlConnection connection =
                new SqlConnection(ConfigurationManager.ConnectionStrings["ADOMovieConnectionString"].ConnectionString))
            {
                // Create an SqlCommand object that will carry out the SQL command
                using (SqlCommand sqlCommand =
                    new SqlCommand("INSERT INTO Movie (Title, ReleaseYear, Genre) VALUES (@Title, @ReleaseYear, @Genre);", connection))
                {
                    // add the parameter values to the command
                    movie.DematerialiseToDbCommand<SqlParameter>(sqlCommand);

                    try
                    {
                        // open connection
                        connection.Open();

                        // run the query
                        sqlCommand.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Something went wrong: {e.Message}");
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }

            Console.WriteLine();
        }

        public static void FindMovieByTitle()
        {
            Console.Write("Enter movie title: ");
            string title = Console.ReadLine();

            using (SqlConnection connection =
                new SqlConnection(ConfigurationManager.ConnectionStrings["ADOMovieConnectionString"].ConnectionString))
            {
                using (SqlCommand sqlCommand =
                    new SqlCommand("SELECT Id, Title, ReleaseYear, Genre FROM Movie WHERE Title = @Title ORDER BY Title;", connection))
                {
                    sqlCommand.Parameters.Add(new SqlParameter("@Title", title));

                    try
                    {
                        connection.Open();

                        SqlDataReader reader = sqlCommand.ExecuteReader();

                        bool found = false;

                        while (reader.Read())
                        {
                            found = true;

                            var movie = Movie.MaterialiseFromReader(reader);

                            Console.WriteLine($"Id: {movie.Id}");
                            Console.WriteLine($"Movie Name: {movie.Title}");
                            Console.WriteLine($"Release Year: {movie.ReleaseYear}");
                            Console.WriteLine($"Genre: {movie.Genre}{Environment.NewLine}");
                        }

                        if (!found)
                        {
                            Console.WriteLine("No movie found with that title.");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Something went wrong: {e.Message}");
                    }
                    finally
                    {
                        connection.Close();
                    }

                    Console.WriteLine("Press a key!");
                    Console.ReadKey();
                }
            }
        }

        public static void FindMovieById()
        {
            Console.Write("Enter movie Id: ");
            int id = int.Parse(Console.ReadLine());

            using (SqlConnection connection =
                new SqlConnection(ConfigurationManager.ConnectionStrings["ADOMovieConnectionString"].ConnectionString))
            {
                using (SqlCommand sqlCommand =
                    new SqlCommand("SELECT Id, Title, ReleaseYear, Genre FROM Movie WHERE Id = @Id;", connection))
                {
                    sqlCommand.Parameters.Add(new SqlParameter("@Id", id));

                    try
                    {
                        connection.Open();

                        SqlDataReader reader = sqlCommand.ExecuteReader();

                        if (reader.Read())
                        {
                            var movie = Movie.MaterialiseFromReader(reader);

                            Console.WriteLine($"Id: {movie.Id}");
                            Console.WriteLine($"Movie Name: {movie.Title}");
                            Console.WriteLine($"Release Year: {movie.ReleaseYear}");
                            Console.WriteLine($"Genre: {movie.Genre}{Environment.NewLine}");
                        }
                        else
                        {
                            Console.WriteLine("No movie found with that Id.");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Something went wrong: {e.Message}");
                    }
                    finally
                    {
                        connection.Close();
                    }

                    Console.WriteLine("Press a key!");
                    Console.ReadKey();
                }
            }
        }

        public static void DeleteMovieById()
        {
            Console.Write("Enter movie Id to delete: ");
            int id = int.Parse(Console.ReadLine());

            using (SqlConnection connection =
                new SqlConnection(ConfigurationManager.ConnectionStrings["ADOMovieConnectionString"].ConnectionString))
            {
                using (SqlCommand sqlCommand =
                    new SqlCommand("DELETE FROM Movie WHERE Id = @Id;", connection))
                {
                    sqlCommand.Parameters.Add(new SqlParameter("@Id", id));

                    try
                    {
                        connection.Open();

                        int rowsAffected = sqlCommand.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("Movie deleted successfully.");
                        }
                        else
                        {
                            Console.WriteLine("No movie found with that Id.");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Something went wrong: {e.Message}");
                    }
                    finally
                    {
                        connection.Close();
                    }

                    Console.WriteLine("Press a key!");
                    Console.ReadKey();
                }
            }
        }

        public static void UpdateMovieById()
        {
            Console.Write("Enter movie Id to update: ");
            int id = int.Parse(Console.ReadLine());

            Console.Write("New Title: ");
            string title = Console.ReadLine();

            Console.Write("New Release Year: ");
            int year = int.Parse(Console.ReadLine());

            Console.Write("New Genre: ");
            string genre = Console.ReadLine();

            using (SqlConnection connection =
                new SqlConnection(ConfigurationManager.ConnectionStrings["ADOMovieConnectionString"].ConnectionString))
            {
                using (SqlCommand sqlCommand =
                    new SqlCommand("UPDATE Movie SET Title = @Title, ReleaseYear = @Year, Genre = @Genre WHERE Id = @Id;", connection))
                {
                    sqlCommand.Parameters.Add(new SqlParameter("@Title", title));
                    sqlCommand.Parameters.Add(new SqlParameter("@Year", year));
                    sqlCommand.Parameters.Add(new SqlParameter("@Genre", genre));
                    sqlCommand.Parameters.Add(new SqlParameter("@Id", id));

                    try
                    {
                        connection.Open();

                        int rowsAffected = sqlCommand.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("Movie updated successfully.");
                        }
                        else
                        {
                            Console.WriteLine("No movie found with that Id.");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Something went wrong: {e.Message}");
                    }
                    finally
                    {
                        connection.Close();
                    }

                    Console.WriteLine("Press a key!");
                    Console.ReadKey();
                }
            }
        }
    }
}



