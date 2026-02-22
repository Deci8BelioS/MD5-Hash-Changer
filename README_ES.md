# MD5 Hash Changer v1.4.2
<div align="right">

[**English**](README.md)

</div>

![Screenshot](https://github.com/Deci8BelioS/MD5-Hash-Changer/blob/main/screenshoot.png?raw=true)

**Cambia el hash MD5 de cualquier archivo** añadiendo bytes calculados al final. Ideal para series/pelis bloqueadas por hash.

## 🚀 Novedades v1.4.x

| ✅ **Nueva** | Descripción |
|-------------|-------------|
| **.NET 8** | Migrado de .NET Framework 4.6 → .NET 8 (más rápido/ligero) |
| **Modo oscuro** | Interfaz completa dark mode |
| **Español** | Traducción total (botones/menús/estados) |
| **MD5 al iniciar** | Sin cálculo al añadir (solo "espera") → **mucho más rápido** |
| **Secuencial** | Procesado 1 archivo a la vez (estable) |
| **Multi-idioma** | ES/EN + detecta idioma del SO automáticamente |
| **ComboBox redondeado** | Selector idiomas con estilo dark |
| **Soporte multilingüe** | Idiomas disponibles Español e Ingles |
| **Nueva lógica MD5** | Ahora calcula los bytes a escribir a final del archivo. |
> **💡 Nota**: Quité cálculo MD5 al arrastrar/añadir. Con series grandes tardaba **minutos**. Ahora calcula solo al pulsar "Iniciar".

## ⚡ Rendimiento
- **1000+ archivos**: UI fluida
- **~2MB** ejecutable único
- **Drag & Drop** instantáneo

## 📱 Características

* Arrastrar archivos/carpetas (recursivo)
* Añadir carpeta (FolderPicker nativo)
* Exportar CSV + copiar filas
* Menú contextual (abrir/borrar)
* Tecla SUPRIMIR para borrar filas
* Barra de progreso + estados visuales
* Ventana centrada + tamaño mínimo

## 💻 Requisitos
- **Windows 10/11** ([.NET 8 incluido](https://dotnet.microsoft.com/es-es/download/dotnet/thank-you/runtime-desktop-8.0.24-windows-x64-installer))
- **~2MB** espacio

## 📥 Descarga
[![Latest Release](https://img.shields.io/github/v/release/Deci8BelioS/MD5-Hash-Changer?color=brightgreen)](https://github.com/Deci8BelioS/MD5-Hash-Changer/releases/latest)

## 🎮 Uso rápido
1. **Descarga** [MD5_Hash_Changer.exe](https://github.com/Deci8BelioS/MD5-Hash-Changer/releases/download/1.4.2/MD5_Hash_Changer.exe)
2. **Ejecuta** 
3. **Arrastra** archivos/carpetas
4. **Iniciar Cambio MD5** → ✅

## 📁 Fork de
Basado en [philip47/MD5-Hash-Changer](https://github.com/philip47/MD5-Hash-Changer)

### Changelog anterior
V1.3 → 
* Mejor Drag&Drop + centrado
* Arrastrar **archivos + carpetas** simultáneo
* Procesado **recursivo** subcarpetas
* Contadores **thread-safe**
