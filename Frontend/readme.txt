I haven't had the time to implement, but the idea would be to store the client id generated on first entry in the cookie, then pull the data endpoint form the second service until it returns the "DataReady" flag with the value of true along with data.

The clarifications:
the first service would get ClientID through query param and the second one through cookie from frontend.