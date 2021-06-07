-----------------------------
INTRODUCTION
-----------------------------
Учитывая, что проект учебный, не буду писать само описание на буржуйском языке, но если это по каким-то причинам входит в задание, могу переписать. 
Заголовки на всякий пожарный пишу на беглом английском.
-----------------------------
SCOPE
-----------------------------
Наша цель состояла в написании очень простого файлового менеджера, который умеет делать следующее:
1. Просмотр файловой структуры.
2. Поддержка копирование файлов, каталогов.
3. Поддержка удаление файлов, каталогов.
4. Получение информации о размерах, системных атрибутов файла, каталога.
5. Постраничный вывод файловой структуры.
6. Наличие конфигурационного файла с различными настройками приложения(вывод количества элементов на страницу,сохранение последнего состояния).
7. Обработка непредвиденных ситуаций(по сути - нужно было поймать как можно больше ошибок при дебаге и "обернуть" их соответствующим образом).
В данном случае часть ошибок будут фиксировать в лог-файле, а другая часть - будет выводиться прямо в консоль. 
8. Поддержка возможности скроллить историю команд (стрелочки вверх, вниз)
--------------------------------
DESIGN AND FEATURES
--------------------------------
1. КОМАНДЫ
Реализованы следующие команды(в коде также есть комменты)

1.1. **cd:{directoryPath}** - переход к директории по пути. При использовании команды на экране консоли будут отображены папки и файлы внутри указанной директории.
Обратите внимание, что при попытке указать путь к файлу таким образом нельзя. Если при обработке команды возникла ошибка(был указан путь к файлу, директории
не существует и т.д.) - на экране консоли появится сообщение:
Empty path detected.Press Enter to get on previous page
Ввод пути к файлу допускается как со строчной так и с прописной буквы.

1.2. **copy:{sourceObject}>{destPath}** - команда копирования папки или файла. При использовании команды sourceObject будет скопирован и вставлен в destPath.
Это, с позволение сказать, приложение само должно определять что ввел пользователь - папку или файл. 
Если все прошло успешно, пользовтаель получит сообщение
Folder D:\Новая папка2 and all its contents has been copied to D:\Новая папка3 - если была папка
File D:\Новая папка2\Новый текстовый документ (2).txt has been copied to D:\ - если был файл
Если при копировании объекта папки destPath нет, она будет создана.

Обратите внимание, что в силу ограничений ОС, некоторые файлы не могут быть скопированы(например, ограничения доступа к файлу из-за того, что какой-то процесс его использует)
В этом случае в консоли будет выведено сообщение Something went wrong! Please refer to the error log file for details, а в лог файл запишется соответствующее сообщение
Например, если попытаться ввести вот такую строку: 
copy:D:\Новый текстовый документ (2).txt>D:\Новый текстовый документ (2).txt
в консоли появится сообщение Something went wrong! Please refer to the error log file for details
в логе будет записано: Cannot create 'D:\Новый текстовый документ (2).txt' because a file or directory with the same name already exists.

1.3. **del:{sourceObject}** - команда удаление папки или файла. При использовании команды sourceObject будет удален(папка или файл).
Честно говоря, я это прям не дебажил, потому что я лох, и каждый раз испытывал дискомфорт в области кохонас каждый раз, когда вводил эту команду.
По общим правилам просто обрабатывал это, т.е. C:\\Windows удалять не пробовал и понятия не имею, что случится...........гы!

1.4. **info:{sourceObject}** - команда вывода информации о директории или файле. Набор аттрибутов можно дополнить, но основной момент тут был в выводе 
размера директории. Там рекурсия, поэтому когда определяется размер оч большого диска(например) иногда нужно подождать до 3-5 сек.

1.5. **стрелки Вправо-Влев**о - будут переключать "страницы", т.е. пагинация реализована так. Отдельной команды перехода между страницами не делал, но могу.
Количество элементов на страницу регламентируется через настройки(Settings.json). Подробнее будет описано ниже.

1.6. Если войти в режим ввода команды и нажать Enter, вы попадете в режим **скроллинга команд**, введенных в рамках предыдущих сессии. 
Стрелками вверх-вниз можно переключить команды, при нажатии Enter вы выполните последнюю команду на экране.

1.7. **клавиша Enter** - вход в командную строку для ввода команд.

1.8. **клавиша Esc** - правильный выход из приложения. Следует использовать эту клавишу, чтоб сохранить все текущие настройки.

**2. СТАРТ ПРИЛОЖЕНИЯ**
Приложению необходим доступ Администратора. Если приложению не хватает привелегий, они будут запрошены при запуске.
Тут все постарался реализовать как было в ТЗ. Сильно с UI не заморачивался, схематично выглядит так:

------------------------
You are here: {Текущая директория}

------------------------
{Подпапки и файлы}
{Текущая страница}

------------------------
{Информация о текущей директории}

------------------------
{Командная строка}

При первом запуске приложения Текущая директория будет диск C:\
При "правильном" выходе из приложения - при нажатии кнопки Escape - текущая директория будет запомнена и следующий старт приложения покажет ее.

**3. НАСТРОЙКИ ПРИЛОЖЕНИЯ**
Настройки хранятся в файле appsettings.json. Этот файл должен быть в корневой директории приложения.
На текущий момент реализованы следующие настройки:
"DefaultDir:"C:\\", - дефолтная директория. 
"CurrentDir":"D:\\", - это ваша текущая директория. Если пустая - приложение покажет диск C
"CurrentPage":1, - это текущая страница. Перезаписывается при правильном выходе из приложения
"PageCounter":10 - количество элементов на страницу. Можно менять, если 10 - мало/много.

Кроме того, у приложения есть еще 2 служебных файла:
ErrorLog.txt - тут фиксируются ошибки, которые возникают при работе приложения. Сейчас я туда включил много всего, но это больше для демонстрации. Можно фиксировать только те ошибки, которые прямо валят приложение. Просто я таких не обнаружил.
history.txt - тут кеш вводимых команд. Нужно, чтоб можно было команды скроллить стрелками вверх-вниз

--------------------------------
OUT OF SCOPE
--------------------------------
1. UI. Тут прям реально особо не старался. Больше интересовал функционал
2. Отстуствие "некоторых" стандартных команд - mkdir,move и др. При желании можно и их прикрутить, но просто консоль не очень приспособлена для полного функционала.
3. Дерево файловый структуры всегда 2 уровня: уровень 1 - текущая директория, уровень 2 - подпки и файлы текущей диреткории
-------------------------------
**APPLICATION STRUCTURE/COMMENTS*тут должна быть диаграмма классов, но это ж md + до сюда все равно никто не дочитает, так что...**
пока в процессе написания, ибо я продолжаю получать ~~на орехи~~ обратную связь
-------------------------------
Приложение состоит из следующих объектов:
CLASS PROGRAM - ОСНОВНОЙ КЛАСС, ТОЧКА ВХОДА
Methods:
- Main - куда ж без него
- ReadObjects - основной метод, с которого начинается рисовалка+перемещение по страницам + вход в парсер команд
- ScrollCommand - метод, который позволяет скроллить команды
- GetObjects - метод, возвращающий все объекты в текущей директории в виде массива. Вспомогательный метод.
- ReadPages - метод, который пишет номер текущей страницы. Вспомогательный метод.
- ChunkToPages - метод, на входе принимающий все объекты текущей директории, а на выходе возвращающий двумерный массив с номером страницы для каждого элемента.
- CommandParser - собсна, парсер команд. Сюда попадает все то, что юзер вводит в командную строку и делающий магию.
- GetObjAfterParser - всопмогательный метод. Просто так, чтоб DRY хотя бы как-то соблюдался.

CLASS SETTINGS
Fields:
- DefaultDir(string) - нужно, если CurrentDir пустое. по дефолту - C:\\
- CurrentDir(string) - текущая диретория. 
- CurrentPage(int) - текущая страница.
- PageCounter(int) - кол-во элементов на странице

CLASS COMMANDPARSER
Fields:
- cd(enum)
- copy(enum)
- move(enum)
- del(enum)
- info(enum)
Methods:
- GetCommand - метод, который принимает то, что ввел юзер, а возвращает команду.
- ValidateCommand - вспомогательный метод, который валидирует команду (сравнивает с перечислениями)

CLASS FOLDERS
Methods:
- Copy - метод копирования директории
- Delete - метод удаления директории
- Info - метод получения информации о директории
- GetSize - метод для определения размера директории

CLASS FILES
Methods:
- Copy - метод копирования файла
- Delete - метод удаления файла
- Info - метод получения информации о файле


