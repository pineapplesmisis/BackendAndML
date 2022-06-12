import hnswlib
import numpy as np
import pickle
import pandas as pd
from img2vec_pytorch import Img2Vec
import torch
import pickle

from sklearn.decomposition import TruncatedSVD

from models import SbertWrapper, mean_pooling, get_embedding_from_link


class HnswWrapper:
    def __init__(self, max_elements=10000, dim_query=192, dim_top=256, space="l2", ef_construction=200, m=32, random_seed=42):
        # Declaring index
        self.query_graph = hnswlib.Index(space='l2', dim=dim_query)
        # Initializing index - the maximum number of elements should be known beforehand
        self.query_graph.init_index(max_elements=max_elements, ef_construction=ef_construction, M=m,
                                                    random_seed=random_seed)

        # Declaring index
        self.top_product_graph = hnswlib.Index(space='l2', dim=dim_top)
        # Initializing index - the maximum number of elements should be known beforehand
        self.top_product_graph.init_index(max_elements=max_elements, ef_construction=ef_construction, M=m,
                                          random_seed=random_seed)

        self.text2vec = SbertWrapper(True)

        self.img2vec = Img2Vec(cuda=True, model="vgg")

        with open('svd_name_query.pickle', 'rb') as f:
            self.svd_name_query = pickle.load(f)

    def make_query_graph(self, file_name, file_db_indexes):
        with open(file_name, 'rb') as f:
            data = np.load(f)

        with open(file_db_indexes, 'rb') as f:
            db_indexes = np.load(f)
        db_indexes = db_indexes[:9779]
        self.query_graph.add_items(data, db_indexes)

    def make_top_product_graph(self, file, db_indexes):
        pass

    def add_product(self):
        pass

    def search_by_query(self, query, k=15):
        encoded_input = self.text2vec.tokenize(query)
        with torch.no_grad():
            model_output = self.text2vec.get_embeddings(encoded_input)
        emb = mean_pooling(model_output, encoded_input['attention_mask'])

        emb = self.svd_name_query.transform(emb.cpu())

        labels, distances = self.query_graph.knn_query(emb, k=k)
        return labels, distances

    def del_product(self):
        pass

    def top_simular_products(self):
        pass