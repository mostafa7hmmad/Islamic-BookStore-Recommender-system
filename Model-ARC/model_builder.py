import tensorflow as tf
from tensorflow.keras.layers import Input, Embedding, Flatten, Concatenate, Dense, Dropout

def build_model(df, all_cat, all_num):
    inputs, embeds = [], []

    for col in all_cat:
        vocab_size = df[col].nunique()
        emb_dim = min(50, max(2, int(round(vocab_size ** 0.25))))
        inp = Input(shape=(1,), name=f"{col}_in")
        x = Embedding(input_dim=vocab_size, output_dim=emb_dim, name=f"{col}_emb")(inp)
        x = Flatten()(x)
        inputs.append(inp)
        embeds.append(x)

    num_in = Input(shape=(len(all_num),), name="num_in")
    inputs.append(num_in)

    x = Concatenate()(embeds + [num_in])
    x = Dense(128, activation="relu")(x)
    x = Dropout(0.3)(x)
    x = Dense(64, activation="relu")(x)
    x = Dropout(0.2)(x)
    x = Dense(32, activation="relu")(x)
    out = Dense(1, activation="linear", name="rating")(x)

    model = tf.keras.Model(inputs=inputs, outputs=out)
    model.compile(optimizer="adam", loss="mse", metrics=["mae"])
    return model
