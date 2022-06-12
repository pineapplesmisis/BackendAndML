'''from fastapi import FastAPI
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
    products = data.getProducts()'''

from data_structures.hnsw import HnswWrapper


hnsw = HnswWrapper()
file_name = "search_hnsw_last.npy"
file_db_indexes = "db_indexes.npy"
hnsw.make_query_graph(file_name, file_db_indexes)
print(hnsw.search_by_query("часики"))
