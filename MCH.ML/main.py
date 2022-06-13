from fastapi import FastAPI
from Models.ProductsIds import  ProductsIds
from data_structures.hnsw import HnswWrapper
import uvicorn


app = FastAPI()

#data = DataRepository()

hnsw = HnswWrapper()
file_name = "search_hnsw.npy"
file_db_indexes = "db_indexes.npy"
file_top="top_product_hnsw.npy"
hnsw.make_query_graph(file_name, file_db_indexes)
hnsw.make_top_product_graph(file_top, file_db_indexes)



@app.get("/")
def read_root():
    return {"Hello": "World"}


@app.get("/api/ml/simiuralProducts/{product_id}/{count}")
def similarProducts(product_id: int,count: int):
    ids = hnsw.top_simular_products(product_id, count)[0].tolist()
    result = ProductsIds()
    result.Ids = ids
    return result

@app.get("/api/ml/searchProducts/{query}/{count}")
def searchByQuery(query: str, count: int):
    ids = hnsw.search_by_query(query, count)[0].tolist()
    result = ProductsIds()
    result.Ids = ids
    return  result






