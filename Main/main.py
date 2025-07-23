import pandas as pd
from sklearn.model_selection import train_test_split
from book_recommender.data_utils import load_encoders, prepare_data
from book_recommender.model_builder import build_model
from book_recommender.train_model import train_model, make_inputs
from book_recommender.recommendation import recommend_books, show_related_books

# ================================
# Load data and encoders
# ================================
df = pd.read_csv("data.csv")
encoders = load_encoders()
title_encoder = encoders["title"]

# ================================
# Prepare data
# ================================
X, y, all_cat, all_num = prepare_data(df)
X_train, X_temp, y_train, y_temp = train_test_split(X, y, test_size=0.2, random_state=42)
X_val, X_test, y_val, y_test = train_test_split(X_temp, y_temp, test_size=0.5, random_state=42)

# ================================
# Build and train model
# ================================
model = build_model(df, all_cat, all_num)
model = train_model(model, X_train, y_train, X_val, y_val, all_cat, all_num)

# ================================
# Evaluate on test set
# ================================
test_data = make_inputs(X_test, all_cat, all_num)
test_loss, test_mae = model.evaluate(test_data, y_test)
print(f"\n🧪 Test RMSE: {test_loss ** 0.5:.3f}, MAE: {test_mae:.3f}")
model.save("t_book_recommender.keras")

# ================================
# Recommend and show similar books
# ================================
top5 = recommend_books(model, df, X_val, title_encoder, all_cat, all_num)
show_related_books(df, title_encoder, top5)
