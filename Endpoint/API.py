from flask import Flask, request, jsonify
import pandas as pd
import joblib
import numpy as np
from tensorflow.keras.models import load_model
from sklearn.neighbors import NearestNeighbors
from utils import standard_scale_age, build_user_inputs, get_recommendations

app = Flask(__name__)

# Load data and model
df = pd.read_csv('model/data.csv')
encoders = joblib.load('model/encoder-full.pkl')
model = load_model('model/t_book_recommender.keras')
book_features = ['topic_idx', 'author_idx', 'category', 'average_rating', 'user_rating_count']
book_feature_df = df.groupby('book_idx')[book_features].first().sort_index()
nn = NearestNeighbors(n_neighbors=6, metric='cosine')
nn.fit(book_feature_df.values)
title_encoder = encoders['title']

@app.route("/recommend", methods=["POST"])
def recommend():
    try:
        data = request.json
        user_vals = {
            'age': standard_scale_age(data['age']),
            'country': encoders['country'].transform([data['country']])[0],
            'gender': encoders['gender'].transform([data['gender']])[0],
            'is_new_muslim': encoders['is_new_muslim'].transform([data['is_new_muslim']])[0],
            'born_muslim': encoders['born_muslim'].transform([data['born_muslim']])[0],
            'education_level': encoders['education_level'].transform([data['education_level']])[0],
            'religious_level': encoders['religious_level'].transform([data['religious_level']])[0],
            'topic_idx': encoders['preferred_topics'].transform([data['preferred_topic']])[0]
        }

        top_titles, related_books_dict = get_recommendations(user_vals, model, df, book_feature_df, nn, title_encoder)

        return jsonify({
            "top_books": top_titles,
            "related_books": related_books_dict
        })

    except Exception as e:
        return jsonify({"error": str(e)}), 500

if __name__ == "__main__":
    app.run(debug=True)
