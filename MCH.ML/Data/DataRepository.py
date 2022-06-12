from dotenv import load_dotenv
from pathlib import Path
import psycopg2
from Models.Product import  Product
import os

class DataRepository:
    def __init__(self) -> None:
        env_path = Path('.') / '.env'
        load_dotenv(dotenv_path=env_path)
        print(env_path)
        DbName = os.getenv("DBNAME")
        Password = os.getenv("Password")
        HostName = os.getenv("HOST")
        print(HostName)
        UserName = os.getenv("User")
        self.connection = psycopg2.connect(dbname='mch', user='mch_user',
                        password='qwert123', host='84.252.138.236')

    def cursor(self):
        return self.connection.cursor()

    def getProducts(self):
        cursor = self.cursor()
        cursor.execute('SELECT * FROM "ProductEntities"')
        records = cursor.fetchall()
        cursor.close()
        return records

    def getProductById(self, productId: int) -> Product:
        cursor = self.cursor()
        cursor.execute(f'SELECT  * FROM "ProductEntities" WHERE "Id"={productId}')
        productEntity = cursor.fetchall()[0]
        product = Product();
        product.id = productEntity[0]
        product.product_name = productEntity[1]
        product.description = productEntity[3]
        product.image_url = productEntity[5]
        cursor.close()
        return product