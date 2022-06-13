import hnswlib
import numpy as np
import pickle
import pandas as pd
from img2vec_pytorch import Img2Vec
import torch
import pickle

from sklearn.decomposition import TruncatedSVD

from Models import SbertWrapper, mean_pooling

from Data.DataRepository import DataRepository


class HnswWrapper:
    """
    HNSW structure for searching by query and similar products
    """
    def __init__(self, max_elements=10000, dim_query=256, dim_top=256, space="l2", ef_construction=200, m=32, random_seed=42):
        # Declaring index
        self.query_graph = hnswlib.Index(space=space, dim=dim_query)
        # Initializing index - the maximum number of elements should be known beforehand
        self.query_graph.init_index(max_elements=max_elements, ef_construction=ef_construction, M=m,
                                    random_seed=random_seed)

        # Declaring index
        self.top_product_graph = hnswlib.Index(space=space, dim=dim_top)
        # Initializing index - the maximum number of elements should be known beforehand
        self.top_product_graph.init_index(max_elements=max_elements, ef_construction=ef_construction, M=m,
                                          random_seed=random_seed)

        self.text2vec = SbertWrapper(True)

        self.img2vec = Img2Vec(cuda=True if torch.cuda.is_available() else False, model="vgg")

        self.db_indexes = None

        self.data = DataRepository()

        with open('svd_name_description_query.pickle', 'rb') as f:
            self.svd_name_description_query = pickle.load(f)

    def make_query_graph(self, file_name, file_db_indexes):
        """
        Builds a graph for queries

        :param file_name: path to file with embeddings
        :param file_db_indexes: path to file with indexes
        """
        with open(file_name, 'rb') as f:
            data = np.load(f)

        with open(file_db_indexes, 'rb') as f:
            db_indexes = np.load(f)

        db_indexes = db_indexes[:data.shape[0]]
        self.query_graph.add_items(data, db_indexes)

    def make_top_product_graph(self, file_name, file_db_indexes):
        """
        Builds a graph for similar products

        :param file_name: path to file with embeddings
        :param file_db_indexes: path to file with indexes
        """
        with open(file_name, 'rb') as f:
            data = np.load(f)

        with open(file_db_indexes, 'rb') as f:
            db_indexes = np.load(f)

        self.db_indexes = db_indexes[:data.shape[0]]

        self.top_product_graph.add_items(data, self.db_indexes)

    def add_product(self):
        pass

    def search_by_query(self, query, k=15):
        """
        Search in the graph by query

        :param query: text of query
        :param k: number of returned items
        :return: array with db_indexes
        """
        encoded_input = self.text2vec.tokenize(query)
        with torch.no_grad():
            model_output = self.text2vec.get_embeddings(encoded_input)
        emb = mean_pooling(model_output, encoded_input['attention_mask'])

        emb = self.svd_name_description_query.transform(emb.cpu())

        labels, distances = self.query_graph.knn_query(emb, k=k)
        return labels

    def del_product(self):
        pass

    def top_simular_products(self, index, k=15):
        """
        Search in the graph by other product

        :param index: index of product
        :param k: number of returned items
        :return: array with db_indexes
        """
        try:
            emb = self.top_product_graph.get_items([index])
            labels, distances = self.top_product_graph.knn_query(emb[0], k=k + 1)
            return labels[:, 1:]
        except RuntimeError:
            product = self.data.getProductById(9175)
            return self.search_by_query(product.product_name + ' ' + product.description, k=k)
