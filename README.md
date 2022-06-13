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

## ML
Для решения задач поиска по названию, поиска похожих товаров и кластеризации мы использовали современные методы машинного обучения. С точки зрения анализа данных 
товар - это набор следующих атрибутов: название, текстовое описание  и изображение . Далее мы преобразуем эти атрибуты в эмбеддинги - векторные представления товаров.

Для получения эмбеддингов из изображений и текста  мы использовали модели [vgg](https://github.com/christiansafka/img2vec) и [sBERT](https://huggingface.co/sberbank-ai/sbert_large_mt_nlu_ru). В файлах [cv.py](https://github.com/pineapplesmisis/Back/blob/main/MCH.ML/Models/cv.py) и [nlp.py](https://github.com/pineapplesmisis/Back/blob/main/MCH.ML/Models/nlp.py) мы написали обёртки на эти модели, для внедрения их в наши алгоритмы поиска.
Таким образом, для каждого товара мы получали 3 ембеддинга, далее конкатенировали их и сжимали при помощи TruncatedSVD
Хранить эмбеддинги мы решили в графовой иерархической структуре данных [HNSW](https://github.com/nmslib/hnswlib). На сегодняшний день эта структура данных является State of the art  для решения наших задач. Преимущество HNSW заключается в том, что :
                                1) Между любыми двумя точками существует короткий путь, или, более формально, матожидание числа кратчайшего пути между двумя случайно                                      выбранными вершинами растёт как O(logN).
                                2) Средняя степень вершины мала.
                                
HNSW позволил нам реализовать быстрый поиск товара с логарифмической асимптотикой. Реализация находится в файле [hnsw.py](https://github.com/pineapplesmisis/Back/blob/main/MCH.ML/data_structures/hnsw.py)

