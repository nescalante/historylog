History Log
==========

Manage entities history logging on database.
All functionality are in the `LogManager` class, which exposes methods to perform logging actions.

## Defining entities ##

**HistoryLog** uses `System.ComponentModel.DataAnnotations` objects to map entitie keys. Entities must have at least one `[Key]` attribute in their properties.
This is an example for an entity to perform logs:

    public class SomeEntity
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public int Number { get; set; }
    }
    
## Logging entities ##

To log some entity just call the `Log` method exposed in `LogManager` class:

    var lm = new LogManager("someApp", connectionString);
    lm.Log(someEntity);
    
You can also register the user that performs action:

    lm.Log(someEntity, someUserName);
    
## Getting history ##

To get history  call the `GetHistory` method exposed in `LogManager` class.
Remember that history are obtained by `Key`, so application gets history of a particular entity:

    var log = lm.GetHistory(someEntity); // returns history of the entity (by key)
