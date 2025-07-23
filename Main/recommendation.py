import numpy as np
from sklearn.neighbors import NearestNeighbors

def recommend_books(model, df, X_val, title_encoder, all_cat, all_num):
    sample_user = X_val.iloc[0:1]
    n_books = df["book_idx"].nunique()

    user_inputs = {f"{col}_in": np.full(n_books, sample_user[col].values[0])
                   for col in all_cat if col != "book_idx"}

    user_inputs["book_idx_in"] = np.arange(n_books)
    user_inputs["author_idx_in"] = df.groupby("book_idx")["author_idx"].first().sort_index().values
    user_inputs["category_in"] = df.groupby("book_idx")["category"].first().sort_index().values

    user_inputs["num_in"] = np.stack([
        np.full(n_books, sample_user["age"].values[0]),
        df.groupby("book_idx")["average_rating"].first().sort_index().values,
        df.groupby("book_idx")["user_rating_count"].first().sort_index().values
    ], axis=1)

    scores = model.predict(user_inputs, batch_size=128).flatten()
    top5 = np.argsort(scores)[-5:][::-1]
    recommended_titles = title_encoder.inverse_transform(top5)

    print(f"\n📚 Top 5 recommendations:")
    for i, t in enumerate(recommended_titles, 1):
        print(f" {i}. {t} (idx={top5[i-1]})")

    return top5

def show_related_books(df, title_encoder, top5):
    book_feature_cols = ["topic_idx", "author_idx", "category", "average_rating", "user_rating_count"]
    book_feature_df = df.groupby("book_idx")[book_feature_cols].first().sort_index()

    nn = NearestNeighbors(n_neighbors=6, metric="cosine")
    nn.fit(book_feature_df.values)

    print("\n📘 Related books:")
    for idx in top5:
        main_title = title_encoder.inverse_transform([idx])[0]
        dists, neigh_idxs = nn.kneighbors([book_feature_df.iloc[idx].values])
        related = neigh_idxs[0][1:]
        print(f"🔹 {main_title}:")
        for ridx in related:
            print(f"    ↳ {title_encoder.inverse_transform([ridx])[0]}")
