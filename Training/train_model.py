import numpy as np
from sklearn.metrics import r2_score, mean_squared_error, mean_absolute_error
from tensorflow import keras

def make_inputs(df_, all_cat, all_num):
    data = {f"{col}_in": df_[col].values for col in all_cat}
    data["num_in"] = df_[all_num].values
    return data

def train_model(model, X_train, y_train, X_val, y_val, all_cat, all_num):
    train_data = make_inputs(X_train, all_cat, all_num)
    val_data = make_inputs(X_val, all_cat, all_num)

    history = model.fit(
        train_data, y_train,
        validation_data=(val_data, y_val),
        epochs=50,
        batch_size=128,
        callbacks=[keras.callbacks.EarlyStopping("val_loss", patience=3, restore_best_weights=True)]
    )

    y_val_pred = model.predict(val_data).flatten()
    print("\n📊 Validation Metrics:")
    print("R² Score:",  r2_score(y_val, y_val_pred))
    print("MSE:     ",  mean_squared_error(y_val, y_val_pred))
    print("MAE:     ",  mean_absolute_error(y_val, y_val_pred))

    return model
