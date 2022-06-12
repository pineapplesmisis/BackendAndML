import torch
torch.cuda.empty_cache()
import gc
gc.collect()
from transformers import AutoTokenizer, AutoModel


class SbertWrapper:
    """
    NLP model for embeddings extraction
    """

    def __init__(self, multitask=True):
        """
        :param multitask: bool, default=True
        The type of the pre-trained model:
        If True, then sberbank-ai/sbert_large_mt_nlu_ru downloaded
        If False, then sberbank-ai/sbert_large_nlu_ru downloaded
        """
        if multitask:
            self.model_name = "sberbank-ai/sbert_large_mt_nlu_ru"
        else:
            self.model_name = "sberbank-ai/sbert_large_nlu_ru"

        self.device = "cuda" if torch.cuda.is_available() else "cpu"

        # Load AutoModel from huggingface model repository
        # https://huggingface.co/sberbank-ai/sbert_large_mt_nlu_ru
        # https://huggingface.co/sberbank-ai/sbert_large_nlu_ru
        self.tokenizer = AutoTokenizer.from_pretrained(self.model_name)
        self.model = AutoModel.from_pretrained(self.model_name).to(self.device)

    def tokenize(self, text):
        """
        Tokenize text

        :param text: data
        :return: encoded data (tokens, token_type_ids, attention_mask)
        """

        return self.tokenizer(text, padding=True, truncation=True, max_length=64, return_tensors='pt')

    def get_embeddings(self, encoded_input):
        """
        Compute token embeddings

        :param encoded_input: tokenized data
        :return: embeddings (last_hidden_state, pooler_output)
        """

        encoded_input = encoded_input.to(self.device)

        with torch.no_grad():
            return self.model(**encoded_input)


def mean_pooling(model_output, attention_mask):
    """
    Mean Pooling - Take attention mask into account for correct averaging

    :param model_output: embeddings (last_hidden_state, pooler_output)
    :param attention_mask: attention_mask from encoded data
    :return: mean token embeddings
    """
    token_embeddings = model_output[0]  # First element of model_output contains all token embeddings
    input_mask_expanded = attention_mask.unsqueeze(-1).expand(token_embeddings.size()).float()
    sum_embeddings = torch.sum(token_embeddings * input_mask_expanded, 1)
    sum_mask = torch.clamp(input_mask_expanded.sum(1), min=1e-9)
    return sum_embeddings / sum_mask
