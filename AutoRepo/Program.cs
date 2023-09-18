using System.Text.RegularExpressions;

Console.WriteLine("Generador de Repositorios");
Console.Write("Ingrese el nombre de la clase: ");
var nombreClase = Console.ReadLine();

// Generar el archivo de la clase de entidad
var entidadPath = Path.Combine("ContaWebApi.DataModel/Entities", $"{nombreClase}.cs");
var entidadContenido = $"using System;\n\nnamespace ContaWebApi.DataModel.Entities\n{{\n    public class {nombreClase}\n    {{\n        // Define los campos y propiedades de la entidad aquí\n    }}\n}}";
File.WriteAllText(entidadPath, entidadContenido);
Console.WriteLine($"Archivo de entidad creado en {entidadPath}");

// Generar el archivo de interfaz del repositorio
var interfazRepository = $"I{nombreClase}Repository.cs";
var pathInterfaz = Path.Combine("ContaWebApi.DataAccess/Core/IRepositories", interfazRepository);
var interfazContenido = $"using ContaWebApi.DataModel.Entities;\n\nnamespace ContaWebApi.DataAccess.Core.IRepositories\n{{\n    public interface {interfazRepository.Replace(".cs", "")} : IRepository<{nombreClase}> \n{{\n        // Implementa tus métodos aquí\n    }}\n}}";
File.WriteAllText(pathInterfaz, interfazContenido);
Console.WriteLine($"Archivo de interfaz creado en {pathInterfaz}");

// Generar el archivo de implementación del repositorio
var implementacionRepository = $"{nombreClase}Repository.cs";
var pathImplementacion = Path.Combine("ContaWebApi.DataAccess/Repositories", implementacionRepository);
var implementacionContenido = $"using ContaWebApi.DataAccess.IRepositories;\nusing ContaWebApi.DataModel.Entities;\n\nnamespace ContaWebApi.DataAccess.Repositories\n{{\n    public class {implementacionRepository.Replace(".cs", "")} : Repository<{nombreClase}>, {interfazRepository.Replace(".cs", "")} \n{{\n        // Implementa tus métodos aquí\n    }}\n}}";
File.WriteAllText(pathImplementacion, implementacionContenido);
Console.WriteLine($"Archivo de implementación creado en {pathImplementacion}");

// Registrar el repositorio en IUnitOfWork.cs
const string unitOfWorkPath = "ContaWebApi.DataAccess/Core/IUnitOfWork.cs";
var unitOfWorkContenido = File.ReadAllText(unitOfWorkPath);

// Buscar la última propiedad con el formato { get; }
const string patron = @"{\s*get;\s*}";
var coincidencias = Regex.Matches(unitOfWorkContenido, patron);
if (coincidencias.Count > 0)
{
    var ultimaCoincidencia = coincidencias[^1];
    var indiceUltimaPropiedad = ultimaCoincidencia.Index + ultimaCoincidencia.Length;

    // Agregar la nueva propiedad después de la última propiedad
    var nuevaPropiedad = $"\n    I{nombreClase}Repository {nombreClase} {{ get; }}";
    unitOfWorkContenido = unitOfWorkContenido.Insert(indiceUltimaPropiedad, nuevaPropiedad);
}
else
{
    // Si no hay propiedades existentes, simplemente agrega la nueva propiedad al final del archivo
    unitOfWorkContenido += $"\n    I{nombreClase}Repository {nombreClase} {{ get; }}";
}

File.WriteAllText(unitOfWorkPath, unitOfWorkContenido);
Console.WriteLine($"Repositorio registrado en {unitOfWorkPath}");

Console.WriteLine("Tarea completada. Presione cualquier tecla para salir.");
Console.ReadKey();
