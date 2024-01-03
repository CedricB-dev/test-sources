import {Route, Routes} from "react-router-dom";
import {Counter} from "./Counter.tsx";
import {Layout} from "./Layout.tsx";
import {Home} from "./Home.tsx";
import {FetchData} from "./FetchData.tsx";

function App() {

  return (
      <Layout>
          <Routes>
              <Route path="/" element={<Home />} />
              <Route path="/counter" element={<Counter />}  />
              <Route path="/fetch-data" element={<FetchData/>} />
          </Routes>
      </Layout>
  )
}

export default App
