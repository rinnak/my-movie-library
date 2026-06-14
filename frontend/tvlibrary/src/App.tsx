import './index.css'
import './App.css'
import {useState} from "react";
import {Sidebar} from "./components/Sidebar/Sidebar.tsx";
import {GenresList} from "./components/GenresList/GenresList.tsx";
import {HeroBanner} from "./components/HeroBanner/HeroBanner.tsx";
import {MovieGrid} from "./components/MovieGrid/MovieGrid.tsx";
import {Header} from "./components/Header/Header.tsx";
import { BrowserRouter, Routes, Route } from "react-router-dom";
import {MovieDetailsPage} from "./components/MovieDetails/MovieDetails.tsx";

function App() {

    const [activeTab, setActiveTab] = useState<string>('main');
    const [sortBy, setSortBy] = useState<'none' | 'rating' | 'year'>('none');
    const [yearRange, setYearRange] = useState<string>('all');
    const [genreRange, setGenre] = useState<string>('all');
    const [searchQuery, setSearchQuery] = useState<string>('');

    const handleTabChange = (tab: string) => {
        setActiveTab(tab);
        searchQuery('');
    }

  return (
      <BrowserRouter>
          <Routes>
              <Route path="/" element={
                  <div className="app-layout">
                      <Sidebar activeTab={activeTab} setActiveTab={setActiveTab} sortBy={sortBy} setSortBy={setSortBy} yearRange={yearRange} setYearRange={setYearRange} genreRange={genreRange} setGenre={setGenre} />
                      <main className="main-content">
                          <Header searchQuery={searchQuery} setSearchQuery={setSearchQuery}/>
                          <HeroBanner />
                          <MovieGrid activeTab={activeTab}
                                     sortBy={sortBy}
                                     yearRange={yearRange}
                                     searchQuery={searchQuery}
                          />
                      </main>
                  </div>
              } />
              <Route path = "/movie/:id" element={<MovieDetailsPage />}/>
          </Routes>
      </BrowserRouter>
  )
}

export default App
