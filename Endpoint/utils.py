import numpy as np

user_cat = ['country', 'gender', 'is_new_muslim', 'born_muslim',
            'education_level', 'religious_level', 'topic_idx']

def standard_scale_age(age):
    return (age - 37.5) / 13.0

def build_user_inputs(user_vals, df):
    n_books = df['book_idx'].nunique()
    inputs = {}
    for col in user_cat:
        inputs[f'{col}_in'] = np.full(n_books, user_vals[col])
    inputs['book_idx_in'] = np.arange(n_books)
    inputs['author_idx_in'] = df.groupby('book_idx')['author_idx'].first().sort_index().values
    inputs['category_in'] = df.groupby('book_idx')['category'].first().sort_index().values
    inputs['num_in'] = np.stack([
        np.full(n_books, user_vals['age']),
        df.groupby('book_idx')['average_rating'].first().sort_index().values,
        df.groupby('book_idx')['user_rating_count'].first().sort_index().values
    ], axis=1)
    return inputs

def get_recommendations(user_vals, model, df, book_feature_df, nn, title_encoder):
    inputs = build_user_inputs(user_vals, df)
    scores = model.predict(inputs, batch_size=128).flatten()
    top5 = np.argsort(scores)[-5:][::-1]
    top_titles = title_encoder.inverse_transform(top5)

    related_books = {}
    for idx in top5:
        title = title_encoder.inverse_transform([idx])[0]
        dists, neigh_idxs = nn.kneighbors([book_feature_df.iloc[idx].values])
        related = neigh_idxs[0][1:]
        related_titles = title_encoder.inverse_transform(related)
        related_books[title] = related_titles.tolist()

    return top_titles.tolist(), related_books
