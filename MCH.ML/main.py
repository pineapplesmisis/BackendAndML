from fastapi import FastAPI
#from Data.DataRepository import DataRepository
from data_structures.hnsw import HnswWrapper
import uvicorn

app = FastAPI()

#data = DataRepository()

hnsw = HnswWrapper()
file_name = "search_hnsw_last.npy"
file_db_indexes = "db_indexes.npy"
hnsw.make_query_graph(file_name, file_db_indexes)


@app.get("/")
def read_root():
    return {"Hello": "World"}


#@app.get("/api/ml/simiuralProducts/{product_id}/{count_id}")
#def read_item(product_id: int,count: int):
    #product = data.getProductById(product_id)
    #return  product

@app.get("/api/ml/searchProducts/{query}")
def read_item(query: str):
    return return hnsw.search_by_query(query)[0].tolist()

if __name__ == "__main__":
    uvicorn.run(app, host="0.0.0.0", port=8000)




