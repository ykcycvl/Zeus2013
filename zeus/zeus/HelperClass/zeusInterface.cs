using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace zeus.HelperClass
{
    /* Интерфейс зевса */

    //Кнопки клавиатуры
    public class zKeybButton
    {
        [XmlAttribute("img")]
        public string img { get; set; } //Изображение кнопки
        [XmlAttribute("location")]
        public string location { get; set; } //Расположение кнопки
        [XmlAttribute("size")]
        public string size { get; set; } //Размеры кнопки
        [XmlAttribute("value")]
        public string value { get; set; } //Значение кнопки

        public zKeybButton()
        {
            img = "";
            location = "";
            size = "";
            value = "";
        }
    }
    //Клавиатура
    public class zKeyboard
    {
        [XmlAttribute("name")]
        public string name { get; set; } //Название клавиатуры
        [XmlAttribute("location")]
        public string location { get; set; } //Расположение клавиатуры
        [XmlAttribute("img")]
        public string img { get; set; } //Бэкграунд клавиатуры

        [XmlArray("keys"), XmlArrayItem("key")]
        public List<zKeybButton> keys { get; set; } //Кнопки, которые входят в клавиатуру

        public zKeyboard()
        {
            name = "";
            location = "";
            img = "";
        }
    }
    //Кнопки
    public class zButton
    {
        [XmlAttribute("location")]
        public string location { get; set; } //Расположение кнопки
        [XmlAttribute("img")]
        public string img { get; set; } //Изображение кнопки
        [XmlAttribute("value")]
        public string value { get; set; } //Значение кнопки
        [XmlAttribute("size")]
        public string size { get; set; } //Размер кнопки
        [XmlAttribute("key")]
        public string key { get; set; } //Ключ кнопки

        public zButton()
        {
            location = "";
            img = "";
            value = "";
            size = "";
            key = "";
        }
    }
    //Надписи
    public class zLabel
    {
        [XmlAttribute("text")]
        public string text { get; set; } //Текст на надписи
        [XmlAttribute("name")]
        public string name { get; set; } //Имя надписи
        [XmlAttribute("location")]
        public string location { get; set; } //Расположение надписи
        [XmlAttribute("size")]
        public string size { get; set; } //Размер надписи
        [XmlAttribute("font-family")]
        public string fontFamily { get; set; } //Название шрифта
        [XmlAttribute("font-size")]
        public string fontSize { get; set; } //Размер шрифта
        [XmlAttribute("font-style")]
        public string fontStyle { get; set; } //Стиль начертания шрифта
        [XmlAttribute("color")]
        public string color { get; set; } //Цвет начертания
        [XmlAttribute("bgcolor")]
        public string bgcolor { get; set; } //Цвет заливки надписи
        [XmlAttribute("text-align")]
        public string textAlign { get; set; } //Выравнивание надписи (по левому краю, по центру, по правому краю)
        [XmlAttribute("text-transform")]
        public string textTransform { get; set; } //Трансформирование текста (верхний регистр, нижний регистр и т.п.)
        [XmlAttribute("with-edit-name")]
        public bool withEditName { get; set; } //Использовать или нет название параметра из файла providers.xml
        [XmlAttribute("with-provider-name")]
        public bool withProviderName { get; set; } //Использовать или нет название параметра из файла providers.xml
        [XmlAttribute("visible")]
        public bool visible { get; set; } //Видим или невидим
        [XmlAttribute("showCurrency")]
        public bool showCurrency { get; set; } //Показывать или не показывать валюту
        [XmlText()]
        public string value { get; set; }

        // Конструктор. Заполняется значениями "по-умолчанию".
        public zLabel()
        {
            text = "";
            name = "";
            location = "";
            size = "";
            fontFamily = "";
            fontSize = "";
            fontStyle = "";
            color = "";
            bgcolor = "transparent";
            textAlign = "";
            textTransform = "";
            withEditName = false;
            withProviderName = false;
            visible = true;
            showCurrency = false;
        }
    }
    //Поле ввода
    public class zInput
    {
        [XmlAttribute("location")]
        public string location { get; set; } //Расположение поля
        [XmlAttribute("size")]
        public string size { get; set; } //Размер поля
        [XmlAttribute("font-family")]
        public string fontFamily { get; set; } //Название шрифта
        [XmlAttribute("font-size")]
        public string fontSize { get; set; } //Размер шрифта
        [XmlAttribute("font-style")]
        public string fontStyle { get; set; } //Стиль начертания шрифта
        [XmlAttribute("color")]
        public string color { get; set; } //Цвет начертания
        [XmlAttribute("bgcolor")]
        public string bgcolor { get; set; } //Цвет заливки поля для ввода
        [XmlAttribute("text-align")]
        public string textAlign { get; set; } //Выравнивание надписи (по левому краю, по центру, по правому краю)
        [XmlAttribute("type")]
        public string type { get; set; } //Тип поля ввода
        [XmlAttribute("mask")]
        public string mask { get; set; } //Маска поля ввода
        [XmlAttribute("visible")]
        public bool visible { get; set; }
        [XmlAttribute("min")]
        public int min { get; set; }
        [XmlAttribute("max")]
        public int max { get; set; }

        public zInput()
        {
            location = "";
            size = "";
            fontFamily = "";
            fontSize = "";
            fontStyle = "";
            color = "";
            bgcolor = "";
            textAlign = "middlecenter";
            type = "";
            visible = true;
            mask = "";
            min = 1;
            max = 20;
        }
    }
    //Логотип провайдера на страницах ввода данных по платежу
    public class zPrvImage
    {
        [XmlAttribute("location")]
        public string location { get; set; } //Расположение логотипчика
        [XmlAttribute("size")]
        public string size { get; set; } // Размер логотипчика

        public zPrvImage()
        {
            location = "";
            size = "";
        }
    }
    //Изображения на форме
    public class zImage
    {
        [XmlAttribute("src")]
        public string src { get; set; } // Путь к картинке
        [XmlAttribute("location")]
        public string location { get; set; } // Позиционирование картинки
        [XmlAttribute("size")]
        public string size { get; set; } // Размер для вывода изображения
        [XmlAttribute("visible")]
        public bool visible { get; set; } // Размер для вывода изображения
        [XmlAttribute("type")]
        public string type { get; set; } // Размер для вывода изображения

        public zImage()
        {
            type = "static";
            visible = true;
            src = "";
            location = "";
            size = "";
        }
    }
    //Формы (описание)
    public class zForm
    {
        [XmlAttribute("bgimg")]
        public string bgimg { get; set; } //Фоновое изображение формы
        [XmlAttribute("inputType")]
        public string inputType { get; set; } //Тип ввода (цифровая клавиатура, буквенно-цифровая и т.п.)
        [XmlAttribute("type")]
        public string type { get; set; } //Тип формы (edit, info и т.п.)
        [XmlAttribute("name")]
        public string name { get; set; } //Имя формы, по которому будет вызвана форма
        [XmlAttribute("id")]
        public string id { get; set; } //ID формы
        [XmlAttribute("timeout")]
        public int timeout { get; set; } //Таймаут отображения формы без действий

        //Логотипчик на форме
        [XmlElement("prvimg")]
        public zPrvImage prvImage { get; set; }
        //Клавиатура формы
        [XmlElement("keyboard")]
        public zKeyboardLink keyboard { get; set; }
        //Список изображений на форме
        [XmlArray("images"), XmlArrayItem("img")]
        public List<zImage> images { get; set; }
        //Список надписей на форме
        [XmlArray("labels"), XmlArrayItem("label")]
        public List<zLabel> labels { get; set; }
        //Список кнопок на форме
        [XmlArray("buttons"), XmlArrayItem("button")]
        public List<zButton> buttons { get; set; }
        //Список кнопок на форме
        [XmlArray("params"), XmlArrayItem("param")]
        public List<zInput> inputParams { get; set; }
        [XmlArray("banners"), XmlArrayItem("flash")]
        public List<zBanner> banners { get; set; } //Список баннеров

        public zForm()
        {
            id = "";
            bgimg = "";
            inputType = "";
            name = "";
            timeout = 30;
        }
    }
    //Клавиатура. Вызов (какую клавиатуру необходимо вызвать)
    public class zKeyboardLink
    {
        [XmlAttribute("name")]
        public string name { get; set; } //Название формы, на которую ссылаемся

        public zKeyboardLink()
        {
            name = "engl";
        }
    }
    //Формы. Вызов (какую форму необходимо вызвать)
    public class zFormLink
    {
        [XmlAttribute("name")]
        public string name { get; set; } //Название формы, на которую ссылаемся
        [XmlAttribute("label")]
        public string label { get; set; } //Текст для label, находящегося в форме. По-умолчанию берется из файла providers.xml
        [XmlAttribute("type")]
        public string type { get; set; } //Текст для label, находящегося в форме. По-умолчанию берется из файла providers.xml
        [XmlAttribute("kvName")]
        public string kvName { get; set; } //Ключ для пары KVpair введенного значения в пользовательской форме

        public zFormLink()
        {
            name = "acceptaccount";
            type = "acceptaccount";
            label = "";
            kvName = "";
        }
    }
    //Опциональный набор форм провайдера
    public class zProvider
    {
        [XmlAttribute("id")]
        public int id { get; set; } //ID провайдера согласно БД зевса
        [XmlArray("forms"), XmlArrayItem("form")]
        public List<zFormLink> forms { get; set; } //собственный набор форм для конкретного провайдера, отличный от "по-умолчанию"

        public zProvider()
        {
            id = 0;
        }
    }
    //Набор форм для каждой группы (категории) провайдеров по-умолчанию
    public class zGroup
    {
        [XmlAttribute("id")]
        public int id { get; set; } //ID группы (категории) провайдеров согласно БД зевса
        [XmlArray("forms"), XmlArrayItem("form")]
        public List<zFormLink> forms { get; set; } //набор форм по-умолчанию для группы (категории) провайдеров

        public zGroup()
        {
            id = 0;
        }
    }

    //Набор форм для раздела "Информация"
    public class zInformation
    {
        [XmlArray("forms"), XmlArrayItem("form")]
        public List<zForm> forms { get; set; }
    }
    //Кнопка провайдера
    public class zMenuItemButton
    {
        [XmlAttribute("id")]
        public int id { get; set; } // ИД провайдера
        [XmlAttribute("img")]
        public string img { get; set; } // Путь к файлу изображения
    }
    //Кнопка группы
    /*public class zGroupButton
    {
        [XmlAttribute("id")]
        public int id { get; set; } // ИД категории (группы) провайдеров
        [XmlAttribute("img")]
        public string img { get; set; } // Путь к файлу изображения
    }*/
    //Флеш-баннер
    public class zBanner
    {
        [XmlAttribute("location")]
        public string location { get; set; } //Расположение баннера
        [XmlAttribute("size")]
        public string size { get; set; } //Размер баннера
        [XmlAttribute("src")]
        public string src { get; set; } //Файл

        public zBanner()
        {
            location = "0;0";
            size = "0;0";
            src = "";
        }
    }
    //Форма главного меню
    public class zMainMenuForm
    {
        [XmlAttribute("count")]
        public short count { get; set; } //Количество кнопок на форме
        [XmlAttribute("startPos")]
        public string startPos { get; set; } //Задание левого верхнего угла, с которого начинается прорисовка кнопок
        [XmlAttribute("dx")]
        public int dx { get; set; } //Дельта X
        [XmlAttribute("dy")]
        public int dy { get; set; } //Дельта Y
        [XmlAttribute("vcount")]
        public short vcount { get; set; } //Количество кнопок по вертикали (сверху вниз)
        [XmlAttribute("hcount")]
        public short hcount { get; set; } //Количество кнопок по горизонтали (слева направо)
        [XmlAttribute("bgimg")]
        public string bgimg { get; set; } //Бэкграунд

        [XmlArray("buttons"), XmlArrayItem("button")]
        public List<zButton> buttons { get; set; } //Список кнопок, входящих в ТОП
        [XmlArray("banners"), XmlArrayItem("flash")]
        public List<zBanner> banners { get; set; } //Список баннеров

        public zMainMenuForm()
        {
            count = 0;
            startPos = "0;0";
            dx = 0;
            dy = 0;
            vcount = 0;
            hcount = 0;
            bgimg = "";
        }
    }

    //Системные форма
    public class zSystemForm
    {
        [XmlAttribute("name")]
        public string name { get; set; } //Название формы
        [XmlAttribute("bgimg")]
        public string bgimg { get; set; } //Фоновое изображение формы
        [XmlElement("top")]
        public zMainMenuForm top { get; set; } //Главное меню: топ
        [XmlElement("group")]
        public zMainMenuForm groupList { get; set; } //Главное меню: категория (группа)
        [XmlElement("list")]
        public zMainMenuForm group { get; set; } //Главное меню: категория (группа)
    }
    //Мета-данные по интерфейсу
    //Версия интерфейса
    public class zIFC
    {
        //Версия интерфейса
        [XmlAttribute("ver")]
        public string version { get; set; }
        //Версия ядра
        [XmlAttribute("corever")]
        public string coreversion { get; set; }

        public zIFC()
        {
            version = "2.0";
            coreversion = "2.0";
        }
    }
    public class zInterface
    {
        [XmlAttribute("id")]
        public short id { get; set; } //Порядковый номер (ID) интерфейса
        [XmlAttribute("name")]
        public string name { get; set; } //Название интерфейса
        [XmlAttribute("folder")]
        public string folder { get; set; } //Папка с файлами интерфейса
        [XmlArray("system_forms"), XmlArrayItem("form")]
        public List<zSystemForm> systemForms { get; set; } //Системные формы
        [XmlArray("forms"), XmlArrayItem("form")]
        public List<zForm> forms { get; set; }
        [XmlArray("keyboards"), XmlArrayItem("keyboard")]
        public List<zKeyboard> keyboards { get; set; }//Клавиатуры
        [XmlArray("customSets"), XmlArrayItem("provider")]
        public List<zProvider> customSets { get; set; } //собственные наборы форм для каждого провайдера
        [XmlArray("userSets"), XmlArrayItem("userSet")]
        public List<zUserSet> userSets { get; set; }
        [XmlArray("groups"), XmlArrayItem("group")]
        public List<zGroup> groups { get; set; } //наборы форм по-умолчанию для групп (категорий) провайдеров
        [XmlArray("groupimages"), XmlArrayItem("group")]
        public List<zMenuItemButton> groupImages { get; set; } //список изображений для групп (категорий) провайдеров
        [XmlArray("prvimages"), XmlArrayItem("prv")]
        public List<zMenuItemButton> prvImages { get; set; } //список изображений для провайдеров
        [XmlElement("information")]
        public zInformation information { get; set; }
        public zInterface()
        {
            id = 5;
            name = "";
            folder = "";
        }
    }

    public class interfaceList
    {
        [XmlElement("ifc")]
        public zIFC ifc { get; set; } //Версия интерфейса
        [XmlArray("interfaceList"), XmlArrayItem("interface")]
        public List<zInterface> zeusinterface { get; set; } //Список интерфейсов
    }

    /*Пользовательские формы*/
    public class zUserSet
    {
        [XmlAttribute("name")]
        public string name { get; set; }
        [XmlArray("forms"), XmlArrayItem("form")]
        public List<zFormLink> forms { get; set; }
    }
}
