from PIL import Image
import requests
from io import BytesIO


async def get_embedding_from_link(link, img2vec):
    """
    Compute photo embeddings

    :param link: link to the photo
    :param img2vec: model
    :return: embeddings
    """
    response = requests.get(link)
    image = Image.open(BytesIO(response.content)).convert('RGB')
    vector = img2vec.get_vec(image)
    return vector
