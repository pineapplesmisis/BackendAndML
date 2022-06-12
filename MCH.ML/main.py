from fastapi import FastAPI
from Data.DataRepository import DataRepository

app = FastAPI()

data = DataRepository()


@app.get("/")
def read_root():
    return {"Hello": "World"}


@app.get("/api/ml/simiuralProducts/{product_id}/{count_id}")
def read_item(product_id: int,count: int):
    product = data.getProductById(product_id)
    return  product

@app.get("/api/ml/searchProducts/{query}")
def read_item(query: str):
    products = data.getProducts()
