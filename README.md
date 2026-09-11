# My Movie Library

Веб-приложение для поиска фильмов и сериалов, управления избранным и просмотренным контентом.

## Технологический стек

* **Frontend:** React, TypeScript, CSS Modules, Vite
* **Backend:** ASP.NET Core, C#, Web API
* **База данных:** SQLite
* **Авторизация:** JWT
* **Пакетный менеджер frontend:** npm

---

## Структура проекта

Проект состоит из двух частей:

```text
my-movie-library/
├── frontend/
│   └── tvlibrary/
│       ├── package.json
│       ├── src/
│       └── ...
│
└── backend/
    └── TVSeriesLibrary/
        └── TVSeriesLibrary/
            ├── TVSeriesLibrary.csproj
            ├── Program.cs
            ├── appsettings.json
            └── ...
```

Frontend и backend запускаются **отдельно**.

---

# Установка необходимых инструментов

Перед запуском проекта необходимо установить:

* Git
* Node.js и npm
* .NET 10 SDK

## 1. Git

Проверить наличие Git:

```bash
git --version
```

Если команда не найдена, установите Git через официальный установщик или Homebrew.

Для macOS с установленным Homebrew:

```bash
brew install git
```

---

## 2. Node.js и npm

Frontend работает на Node.js и npm.

Проверить их наличие:

```bash
node --version
npm --version
```

Если команды не найдены, установите Node.js.

При наличии Homebrew:

```bash
brew install node
```

После установки снова проверьте:

```bash
node --version
npm --version
```

---

## 3. .NET 10 SDK

Backend написан на ASP.NET Core и использует **.NET 10**.

Это можно проверить в файле:

```text
backend/TVSeriesLibrary/TVSeriesLibrary/TVSeriesLibrary.csproj
```

В проекте используется:

```xml
<TargetFramework>net10.0</TargetFramework>
```

### Установка на macOS

Для Mac с процессором Apple Silicon (M1/M2/M3/M4) используется версия **Arm64**.

Проверить архитектуру компьютера:

```bash
uname -m
```

Для Apple Silicon результат будет:

```text
arm64
```

Если установлен Homebrew, .NET SDK можно установить командой:

```bash
brew install --cask dotnet-sdk
```

После установки **закройте Terminal и откройте его заново**.

Проверьте установку:

```bash
dotnet --version
```

Также можно проверить установленные SDK:

```bash
dotnet --list-sdks
```

В списке должна присутствовать версия **10.x**.

> Важно: для запуска backend нужен именно **.NET SDK**, а не только .NET Runtime.

---

# Клонирование проекта

Склонируйте репозиторий:

```bash
git clone https://github.com/rinnak/my-movie-library.git
```

Перейдите в папку проекта:

```bash
cd my-movie-library
```

Проверить содержимое проекта можно командой:

```bash
ls
```

Должны присутствовать папки:

```text
frontend
backend
```

---

# Запуск Backend

Backend работает на ASP.NET Core и запускается отдельно от frontend.

Откройте **новую вкладку Terminal** (`⌘ + T`).

Перейдите в папку backend:

```bash
cd ~/my-movie-library/backend/TVSeriesLibrary/TVSeriesLibrary
```

Восстановите зависимости проекта:

```bash
dotnet restore
```

Запустите backend:

```bash
dotnet run
```

При успешном запуске появится сообщение:

```text
Now listening on: http://localhost:5005
Application started.
```

Backend будет доступен по адресу:

```text
http://localhost:5005
```

**Не закрывайте этот терминал и не нажимайте `Ctrl + C`**, пока работаете с приложением. Это остановит backend.

---


# Запуск Frontend

Frontend находится не непосредственно в папке `frontend`, а в:

```text
frontend/tvlibrary
```

Поэтому команды npm необходимо выполнять именно там.

Перейдите в папку:

```bash
cd ~/my-movie-library/frontend/tvlibrary
```

Установите зависимости:

```bash
npm install
```

После установки запустите frontend:

```bash
npm run dev
```

В терминале появится адрес примерно такого вида:

```text
Local: http://localhost:5173/
```

Откройте этот адрес в браузере.


Правильная папка:

```text
~/my-movie-library/frontend/tvlibrary
```

---


# Одновременный запуск приложения

Для полноценной работы приложения должны быть запущены **два процесса**.

### Terminal 1 — Backend

```bash
cd ~/my-movie-library/backend/TVSeriesLibrary/TVSeriesLibrary
dotnet run
```

Ожидаемый адрес:

```text
http://localhost:5005
```

### Terminal 2 — Frontend

```bash
cd ~/my-movie-library/frontend/tvlibrary
npm run dev
```

После запуска откройте адрес, который покажет Vite, например:

```text
http://localhost:5173
```

Итоговая схема работы:

```text
                Браузер
                   │
                   ▼
          React / Vite Frontend
             localhost:5173
                   │
                   │ API-запросы
                   ▼
          ASP.NET Core Backend
             localhost:5005
                   │
                   ▼
               SQLite
```

---

# Быстрый запуск после установки

Если все необходимые инструменты уже установлены, для запуска проекта достаточно открыть **два терминала**.

### Терминал 1 — Backend

```bash
cd ~/my-movie-library/backend/TVSeriesLibrary/TVSeriesLibrary
dotnet run
```

### Терминал 2 — Frontend

```bash
cd ~/my-movie-library/frontend/tvlibrary
npm run dev
```

После этого открыть адрес frontend, который будет указан Vite.

---
