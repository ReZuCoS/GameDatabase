# **Game database project**

## Desctiption

**Game Database** is my pet project that allows you to create lists of games you've completed and are planning to complete, as well as leave a rating of the game. The project is being created for educational purposes and generally clones the concept of the rawg.io website.

## Database

I chose **PostgreSQL** as my database management system because it is a robust and scalable open source DBMS. In addition to its efficiency in processing large amounts of data, it complies with a large number of SQL standards, which makes it possible to quickly migrate the database to another DBMS if needed.

### ER-diagram

![Entity relationship diagram image](.media/Images/GameDatabase.svg)

### Database Connection string

To connect the application to the database, create a **appsettings.ConnectionString.json** file in ./API directory with the following content before building the project: 

```json
{
    "ConnectionStrings": {
        "DefaultConnection": "Server=host; Port=port; Database=database_name; User Id=user; Password=your_password;"
    }
}
```

*This description will be updated as the project develops.*
