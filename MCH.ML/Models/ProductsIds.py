from typing import List
from pydantic import BaseModel

class ProductsIds(BaseModel):
    Ids: List[int] = []