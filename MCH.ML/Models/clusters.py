from sklearn.cluster import DBSCAN
import numpy as np

from sklearn.manifold import TSNE


class ClusteringWrapper:
    def __init__(self, file_name, file_db_indexes):
        with open(file_name, 'rb') as f:
            data = np.load(f)
        self.tsne = TSNE(n_components=2, random_state=42)
        data = self.tsne.fit_transform(data)
        self.clustering = DBSCAN(eps=8, min_samples=10).fit(data)

    def get_labels(self):
        labels = self.clustering.labels_
        decription = {
            0: "Мясная продукция",
            1: "Одежда",
            2: "Кольца",
            3: "Часы",
            4: "Одежда делового стиля",
            5: "Напитки",
            6: "Знаки",
            7: "Серьги",
            8: "Акты",
            9: "Схемы",
            10: "Штампы",
        }
        return labels, decription