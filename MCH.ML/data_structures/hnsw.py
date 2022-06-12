import hnswlib
import numpy as np
import pickle
import pandas as pd
from img2vec_pytorch import Img2Vec
import torch

from sklearn.decomposition import TruncatedSVD

from models import SbertWrapper, mean_pooling, get_embedding_from_link

class HnswWrapper:
    """

    """
    def __int__(self, space="l2", dim, max_elements, ef_construction=200, m=32, random_seed=42):
        """
        :type max_elements:
        :param space:
        :param dim:
        :param max_elements:
        :param ef_construction:
        :param m:
        :param random_seed:
        :return:
        """
        # Declaring index
        self.query_graph = hnswlib.Index(space='l2', dim=dim)
        # Initializing index - the maximum number of elements should be known beforehand
        self.query_graph.init_index(max_elements=max_elements, ef_construction=ef_construction, M=m,
                                    random_seed=random_seed)

        # Declaring index
        self.top_product_graph = hnswlib.Index(space='l2', dim=dim)
        # Initializing index - the maximum number of elements should be known beforehand
        self.top_product_graph.init_index(max_elements=max_elements, ef_construction=ef_construction, M=m,
                                    random_seed=random_seed)

        self.text2vec = SbertWrapper(True)

        self.img2vec = Img2Vec(cuda=True, model="vgg")

    def make_query_graph(self, file_name, bd_indexes):
        with open(file_name, 'rb') as f:
            data = np.load(f)

        self.query_graph.add(data, bd_indexes)

    def make_top_product_graph(self, file, bd_indexes):
        pass

    def add_product(self):
        pass

    def search_by_query(self, query, k=15):
        encoded_input = self.text2vec.tokenize(query)
        with torch.no_grad():
            model_output = self.text2vec.get_embeddings(encoded_input)
        emb = mean_pooling(model_output, encoded_input['attention_mask'])[0]
        labels, distances = self.query_graph.knn_query(emb, k=k)
        return labels

    def del_product(self):
        pass

    def top_simular_products(self):
        pass
