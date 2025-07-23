import pandas as pd
import joblib

def load_encoders(path="encoder-full.pkl"):
    return joblib.load(path)

def prepare_data(df):
    df = df.rename(columns={
        "title":   "book_idx",
        "author":  "author_idx",
        "preferred_topics": "topic_idx"
    })

    cat_cols = [
        "book_idx", "topic_idx", "author_idx",
        "country", "gender", "is_new_muslim", "born_muslim",
        "education_level", "religious_level", "category"
    ]
    num_cols = ["age", "average_rating", "user_rating_count"]
    target   = "rating"

    X = df[cat_cols + num_cols]
    y = df[target]
    
    return X, y, cat_cols, num_cols
