    В этом репозитории находятся два модуля нашего продукта: модуль парсера и модуль API.
## Парсер

    Парсер - background worker, который в фоновом режиме парсит сайты. Список сайтов находится в БД. Также парсер работает с XMl документами, в которых описаны правила парсинга в формате XPath или RegExp (регулярные выражения). Данный сервис по сути является шаблонным, так как для добавления нового сайта для парсинга в общем случае нет небоходимости менять программный код, а достаточно лишь добавить Xml документ.

## API
API работает с базой данных, в которую сохраняются товары с сайтов. API представляет методы для получения списка компания, списка товаров (по разным условиям), а также дает возможность добавлять новые компании. 

И Парсер и API написании на языке C# (.NET5) и являются кроссплатформенными, то есть могут быть запущены в различных окружениям. 
Также подготовлены файлы Dockerfile и docker-compose.yml, которые позволяют запустить данные сервисы  в две команды:

### docker-compose build
### docker-compose up -d

В docker-compose также добавлен образ веб-сервера Nginx и все запросы к API будут идти через него.

Сервисы работают с базой данных Postgre. Для того чтобы создать нужную структуре в базе данных также достаточно двух команд:

### dotnet ef migrations add "add autoincrements 2" --project MCH.API
### dotnet ef database update "add autoincrements 2" --project MCH.API

Эти команды запустят миграции в базе данных на основании моделей в программном кодею.
! Перед запускам этих комманд необходимо добавить строку подключения к базе данных в конфигурационный файл appsettings.json проекта MCH.API
____
## ML
Для решения задач поиска по названию, поиска похожих товаров и кластеризации мы использовали современные методы машинного обучения. С точки зрения анализа данных 
товар - это набор следующих атрибутов: название, текстовое описание  и изображение . Далее мы преобразуем эти атрибуты в эмбеддинги - векторные представления товаров.

SbertWrapper - предобученный BERT, принимающий на вход текстовые данные и возвращающий эмбеддинги [nlp.py](https://github.com/pineapplesmisis/BackendAndML/blob/main/MCH.ML/Models/nlp.py)
В файле [cv.py](https://github.com/pineapplesmisis/BackendAndML/blob/main/MCH.ML/Models/cv.py) находится модель, преобразующая изображения(в виде ссылок) в эмбеддинги
HnswWrapper - класс, строющий 2 иерархических графа HNSW для решения задач поиска товаров по текстовому запросу и поиска похожих товаров [hnsw.py](https://github.com/pineapplesmisis/Back/blob/main/MCH.ML/data_structures/hnsw.py)
ClustersWrapper - класс, разделяющий данные на кластеры с помощью DBSCAN  [clusters.py](https://github.com/pineapplesmisis/BackendAndML/blob/main/MCH.ML/Models/clusters.py)
