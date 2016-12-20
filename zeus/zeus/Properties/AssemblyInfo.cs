using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Zeus - Processing Core")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("IPAYBOX Processing")]
[assembly: AssemblyProduct("Zeus")]
[assembly: AssemblyCopyright("Copyright © ipaybox.ru, © Kashlev Dmitry 2009-2013")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]


// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("3dd92143-26b4-4896-aae6-9c762c1931f8")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("2.0.1.12")]
[assembly: AssemblyFileVersion("2.0.1.12")]

/*
 21 09 2009 [!] entermoney: BillError->Create pay
 22 09 2009 [+] FileRequest > FileResponse 3->4
 06 10 2009 [+] Номерные емкости
 12 10 2009 [!] Номерные емкости
 * [+] Provider XML add attribute ignore_prange = "true" - не проверяется воодимые данные, иначе проверяются.
 * [!] При смене провайдера по номерной емкости - не менялась PROVIDER_XML
 09 09 2014
 * [!] Исправлен опрос купюрника на предмет снятия стекера: обработка производится в параллельном потоке, а не в основном
 * [!] Исправлена перерисовка поля ввода в форме edit - вывод изображением на заднем фоне, вместо контрола, для
 *      ускорения отрисовки
 * [!] Оптимизирована работа с памятью - при переходе из формы в форму нет утечек
 
 */