using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Lab_3.Models;
using System.IO;

namespace Lab_3.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class api : ControllerBase
    {
        [HttpPost, Route("compress/{nombre}")]
        public void Compresion(IFormFile file, string nombre)
        {
            ArbolHuffman.Comprimir(file, nombre);

            var newFile = new FileInfo(Path.Combine(Environment.CurrentDirectory, "compress", $"{nombre}.huff"));

            NodoArchivo.ManejarCompressions(
                new NodoArchivo
                {
                    Algoritmo = "Huffman",
                    NombreOriginal = file.FileName,
                    Nombre = $"{nombre}.huff",
                    RutaArchivo = Path.Combine(Environment.CurrentDirectory, "compress", $"{nombre}.huff"),
                    RazonCompresion = (double)newFile.Length / (double)file.Length,
                    FactorCompresion = (double)file.Length / (double)newFile.Length,
                    Porcentaje = 100 - (((double)newFile.Length / (double)file.Length) * 100)
                });
        }

        [HttpPost, Route("decompress")]
        public void Desompresion(IFormFile file)
        {
            var Archivos = NodoArchivo.CargarHistorial();

            var Original = Archivos.Find(c => Path.GetFileNameWithoutExtension(c.Nombre) == Path.GetFileNameWithoutExtension(file.FileName));

            var path = ArbolHuffman.Descomprimir(file, Original.NombreOriginal);

            var newFile = new FileInfo(path);

            NodoArchivo.ManejarCompressions(
                new NodoArchivo
                {
                    Algoritmo = "Huffman",
                    NombreOriginal = Original.NombreOriginal,
                    Nombre = file.FileName,
                    RutaArchivo = path,
                    RazonCompresion = 0,
                    FactorCompresion = 0,
                    Porcentaje = 0
                });
        }

        [HttpGet, Route("compressions")]
        public List<NodoArchivo> Get()
        {
            var compresiones = new List<NodoArchivo>();
            var logicaLIFO = new Stack<NodoArchivo>();
            var Linea = string.Empty;

            using (var Reader = new StreamReader(Path.Combine(Environment.CurrentDirectory, "compressions.txt")))
            {
                while (!Reader.EndOfStream)
                {
                    var historialtemp = new NodoArchivo();
                    Linea = Reader.ReadLine();
                    historialtemp.Algoritmo = Linea;
                    Linea = Reader.ReadLine();
                    historialtemp.NombreOriginal = Linea;
                    Linea = Reader.ReadLine();
                    historialtemp.Nombre = Linea;
                    Linea = Reader.ReadLine();
                    historialtemp.RutaArchivo = Linea;
                    Linea = Reader.ReadLine();
                    historialtemp.RazonCompresion = Convert.ToDouble(Linea);
                    Linea = Reader.ReadLine();
                    historialtemp.FactorCompresion = Convert.ToDouble(Linea);
                    Linea = Reader.ReadLine();
                    historialtemp.Porcentaje = Convert.ToDouble(Linea);
                    logicaLIFO.Push(historialtemp);
                }
            }

            while (logicaLIFO.Count != 0)
            {
                compresiones.Add(logicaLIFO.Pop());
            }

            return compresiones;
        }
    }
}
